using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.Leaderboards;

namespace ChallengeTracker.Api.Api.Endpoints;

public static class LeaderboardEndpoints
{
  public static void MapLeaderboardEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/leaderboards").WithTags("Leaderboards");

    // GET /leaderboards/challenges/{id}?period=total&top=10
    group.MapGet("/challenges/{id:guid}", async (
        Guid id,
        ILeaderboardService service,
        string? period,
        int? top) =>
    {
      // Only "total" is supported in phase 5 — daily / weekly come later.
      var requested = (period ?? "total").ToLowerInvariant();
      if (requested != "total")
      {
        return Results.Problem(
            title: "Unsupported period",
            detail: "Only 'total' is supported in this version.",
            statusCode: StatusCodes.Status400BadRequest);
      }

      var board = await service.GetTotalAsync(id, top ?? 10);
      return board is null ? Results.NotFound() : Results.Ok(board);
    })
    .Produces<LeaderboardResponseDto>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);
  }
}
