using Microsoft.EntityFrameworkCore;

using ChallengeTracker.Api.Infrastructure;

namespace ChallengeTracker.Api.Api.Endpoints;

public static class HealthEndpoints
{
  public static void MapHealthEndpoints(this IEndpointRouteBuilder app)
  {
    // Liveness — process is alive and responding.
    app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
        .WithTags("Health");

    // Readiness — checks DB connectivity. 503 if the DB isn't reachable.
    app.MapGet("/ready", async (ApplicationDbContext db) =>
    {
      try
      {
        var canConnect = await db.Database.CanConnectAsync();
        if (!canConnect)
          return Results.Problem(title: "Database not reachable", statusCode: StatusCodes.Status503ServiceUnavailable);

        var pending = await db.Database.GetPendingMigrationsAsync();
        if (pending.Any())
          return Results.Problem(title: "Pending migrations", statusCode: StatusCodes.Status503ServiceUnavailable);

        return Results.Ok(new { status = "ready" });
      }
      catch (Exception ex)
      {
        return Results.Problem(title: "Readiness check failed", detail: ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
      }
    })
    .WithTags("Health");
  }
}
