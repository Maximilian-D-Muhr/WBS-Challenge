using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

using ChallengeTracker.Api.Api.Filters;
using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.ProgressEntries;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Api.Endpoints;

public static class ProgressEntryEndpoints
{
  public static void MapProgressEntryEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/progress-entries").WithTags("ProgressEntries");

    // POST /progress-entries — log progress for a challenge
    group.MapPost("/", [Authorize] async (
        CreateProgressEntryDto dto,
        IProgressEntryService service,
        ClaimsPrincipal user) =>
    {
      var userId = GetUserId(user);
      if (userId is null) return Results.Unauthorized();

      try
      {
        var entry = await service.LogAsync(userId.Value, dto);
        return Results.Created($"/progress-entries/{entry.Id}", MapResponse(entry));
      }
      catch (ArgumentException ex)
      {
        return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
      }
      catch (InvalidOperationException ex)
      {
        // The 1-per-day rule lives here.
        return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status409Conflict);
      }
    })
    .WithValidation<CreateProgressEntryDto>()
    .RequireRateLimiting("progress-post")
    .Produces<ProgressEntryResponseDto>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status409Conflict);

    // PATCH /progress-entries/{id} — owner-only, within 24h
    group.MapPatch("/{id:guid}", [Authorize] async (
        Guid id,
        UpdateProgressEntryDto dto,
        IProgressEntryService service,
        ClaimsPrincipal user) =>
    {
      var userId = GetUserId(user);
      if (userId is null) return Results.Unauthorized();

      try
      {
        var entry = await service.UpdateAsync(userId.Value, id, dto);
        return entry is null ? Results.NotFound() : Results.Ok(MapResponse(entry));
      }
      catch (ArgumentException ex)
      {
        return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
      }
    })
    .WithValidation<UpdateProgressEntryDto>()
    .Produces<ProgressEntryResponseDto>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound);

    // DELETE /progress-entries/{id} — owner-only, within 24h
    group.MapDelete("/{id:guid}", [Authorize] async (
        Guid id,
        IProgressEntryService service,
        ClaimsPrincipal user) =>
    {
      var userId = GetUserId(user);
      if (userId is null) return Results.Unauthorized();

      try
      {
        var ok = await service.DeleteAsync(userId.Value, id);
        return ok ? Results.NoContent() : Results.NotFound();
      }
      catch (ArgumentException ex)
      {
        return Results.Problem(title: ex.Message, statusCode: StatusCodes.Status400BadRequest);
      }
    })
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound);

    // GET /progress-entries/me/today?challengeId=...
    // Convenience endpoint so the frontend can render today's entry without
    // fetching the whole list.
    group.MapGet("/me/today", [Authorize] async (
        Guid challengeId,
        IProgressEntryService service,
        ClaimsPrincipal user) =>
    {
      var userId = GetUserId(user);
      if (userId is null) return Results.Unauthorized();

      var entry = await service.GetMyTodayAsync(userId.Value, challengeId);
      return entry is null ? Results.NoContent() : Results.Ok(MapResponse(entry));
    })
    .Produces<ProgressEntryResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status401Unauthorized);

    // GET /progress-entries/me?challengeId=... — full personal history
    group.MapGet("/me", [Authorize] async (
        Guid challengeId,
        IProgressEntryService service,
        ClaimsPrincipal user) =>
    {
      var userId = GetUserId(user);
      if (userId is null) return Results.Unauthorized();

      var list = await service.ListMineAsync(userId.Value, challengeId);
      return Results.Ok(list.Select(MapResponse).ToList());
    })
    .Produces<List<ProgressEntryResponseDto>>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status401Unauthorized);
  }

  private static Guid? GetUserId(ClaimsPrincipal user)
  {
    var raw = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
              ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(raw, out var id) ? id : null;
  }

  private static ProgressEntryResponseDto MapResponse(ProgressEntry e) => new(
      e.Id,
      e.UserId,
      e.ChallengeId,
      e.Amount,
      e.Note,
      e.LoggedAt,
      e.CreatedAt
  );
}
