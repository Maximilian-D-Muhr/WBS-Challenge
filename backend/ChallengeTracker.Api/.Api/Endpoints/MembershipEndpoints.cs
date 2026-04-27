using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using ChallengeTracker.Api.Api.Filters;
using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.Memberships;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Api.Endpoints;

public static class MembershipEndpoints
{
  public static void MapMembershipEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/memberships").WithTags("Memberships");

    // POST /memberships — join a challenge
    group.MapPost("/", [Authorize] async (
        CreateMembershipDto dto,
        IMembershipService service,
        ClaimsPrincipal user) =>
    {
      var userId = GetUserId(user);
      if (userId is null) return Results.Unauthorized();

      try
      {
        var membership = await service.JoinAsync(userId.Value, dto.ChallengeId);
        return Results.Created($"/memberships/{membership.Id}", MapResponse(membership));
      }
      catch (ArgumentException ex)
      {
        return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
      }
    })
    .WithValidation<CreateMembershipDto>()
    .Produces<MembershipResponseDto>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized);

    // DELETE /memberships/{id} — user leaves a challenge
    group.MapDelete("/{id:guid}", [Authorize] async (
        Guid id,
        IMembershipService service,
        ClaimsPrincipal user) =>
    {
      var userId = GetUserId(user);
      if (userId is null) return Results.Unauthorized();

      var ok = await service.LeaveAsync(userId.Value, id);
      if (!ok)
        return Results.Problem(title: "Membership not found or not yours", statusCode: StatusCodes.Status404NotFound);

      return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound);

    // PATCH /memberships/{id} — owner approves a pending membership
    group.MapPatch("/{id:guid}", [Authorize] async (
        Guid id,
        IMembershipService service,
        ClaimsPrincipal user) =>
    {
      var ownerId = GetUserId(user);
      if (ownerId is null) return Results.Unauthorized();

      try
      {
        var membership = await service.ApproveAsync(ownerId.Value, id);
        if (membership is null)
          return Results.Problem(title: "Not found or not the challenge owner", statusCode: StatusCodes.Status404NotFound);

        return Results.Ok(MapResponse(membership));
      }
      catch (ArgumentException ex)
      {
        return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
      }
    })
    .Produces<MembershipResponseDto>()
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound);
  }

  private static Guid? GetUserId(ClaimsPrincipal user)
  {
    var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    return Guid.TryParse(sub, out var id) ? id : null;
  }

  private static MembershipResponseDto MapResponse(Membership m) =>
      new(m.Id, m.UserId, m.ChallengeId, m.Status, m.JoinedAt);
}
