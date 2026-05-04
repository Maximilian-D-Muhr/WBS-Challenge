using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using ChallengeTracker.Api.Api.Filters;
using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.Auth;

namespace ChallengeTracker.Api.Api.Endpoints;

public static class AuthEndpoints
{
  public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/auth").WithTags("Auth");

    group.MapPost("/register", async (RegisterRequestDto request, IAuthService authService) =>
    {
      var (success, errors) = await authService.RegisterAsync(request);
      if (!success)
        return Results.Problem(
            title: "Registration failed",
            detail: string.Join("; ", errors),
            statusCode: StatusCodes.Status400BadRequest);

      return Results.Ok();
    })
    .WithValidation<RegisterRequestDto>()
    .Produces(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest);

    group.MapPost("/login", async (LoginRequestDto request, IAuthService authService) =>
    {
      var result = await authService.LoginAsync(request);
      if (result is null)
        return Results.Problem(
            title: "Invalid credentials",
            statusCode: StatusCodes.Status401Unauthorized);

      return Results.Ok(result);
    })
    .WithValidation<LoginRequestDto>()
    .Produces<AuthResponseDto>()
    .ProducesProblem(StatusCodes.Status401Unauthorized);

    group.MapGet("/me", [Authorize] async (ClaimsPrincipal principal, IAuthService authService) =>
    {
      var userIdClaim = principal.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
      if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();

      var me = await authService.GetCurrentUserAsync(userId);
      if (me is null)
        return Results.NotFound();

      return Results.Ok(me);
    })
    .Produces<MeResponseDto>()
    .ProducesProblem(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound);
  }
}
