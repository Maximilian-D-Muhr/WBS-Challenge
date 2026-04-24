using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using ChallengeTracker.Api.Api.Filters;
using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.Challenges;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Api.Endpoints;

public static class ChallengeEndpoints
{
  public static void MapChallengeEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/challenges").WithTags("Challenges");

    // POST /challenges — owner = current user
    group.MapPost("/", [Authorize] async (
        CreateChallengeDto dto,
        IChallengeService service,
        HttpContext context,
        ClaimsPrincipal user) =>
    {
      var userId = GetUserId(user);
      if (userId is null) return Results.Unauthorized();

      if (dto.EndDate < dto.StartDate)
        return Results.Problem(
            title: "Invalid date range",
            detail: "EndDate must be on or after StartDate.",
            statusCode: StatusCodes.Status400BadRequest);

      var challenge = await service.CreateAsync(userId.Value, dto);
      var response = MapResponse(challenge);

      var location = $"{context.Request.Scheme}://{context.Request.Host}/challenges/{challenge.Id}";
      return Results.Created(location, response);
    })
    .WithValidation<CreateChallengeDto>()
    .Produces<ChallengeResponseDto>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized);

    // GET /challenges?visibility=Public&status=Open
    group.MapGet("/", async (
        IChallengeService service,
        ChallengeVisibility? visibility,
        ChallengeStatus? status) =>
    {
      var list = await service.ListAsync(visibility, status);
      var response = list.Select(MapResponse).ToList();
      return Results.Ok(response);
    })
    .Produces<List<ChallengeResponseDto>>();

    // GET /challenges/{id}
    group.MapGet("/{id:guid}", async (
        Guid id,
        IChallengeService service,
        ClaimsPrincipal user) =>
    {
      var challenge = await service.GetAsync(id);
      if (challenge is null)
        return Results.Problem(title: "Challenge not found", statusCode: StatusCodes.Status404NotFound);

      // Private challenges are only visible to the owner or active members
      if (challenge.Visibility == ChallengeVisibility.Private)
      {
        var userId = GetUserId(user);
        var isOwner = userId is not null && challenge.OwnerId == userId;
        var isMember = userId is not null && challenge.Memberships.Any(m => m.UserId == userId);
        if (!isOwner && !isMember)
          return Results.Problem(title: "Forbidden", statusCode: StatusCodes.Status403Forbidden);
      }

      return Results.Ok(MapResponse(challenge));
    })
    .Produces<ChallengeResponseDto>()
    .ProducesProblem(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status404NotFound);
  }

  private static Guid? GetUserId(ClaimsPrincipal user)
  {
    var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    return Guid.TryParse(sub, out var id) ? id : null;
  }

  private static ChallengeResponseDto MapResponse(Challenge c) =>
      new(
          c.Id,
          c.OwnerId,
          c.Owner?.DisplayName ?? string.Empty,
          c.Title,
          c.Description,
          c.Visibility,
          c.Status,
          c.StartDate,
          c.EndDate,
          c.UnitLabel,
          c.CreatedAt);
}
