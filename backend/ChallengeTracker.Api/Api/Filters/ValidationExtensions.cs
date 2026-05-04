namespace ChallengeTracker.Api.Api.Filters;

public static class ValidationExtensions
{
  // Wires a ValidationFilter<T> onto an endpoint.
  // Usage: group.MapPost(...).WithValidation<CreateChallengeDto>();
  public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
      where T : class
  {
    return builder.AddEndpointFilter<ValidationFilter<T>>();
  }
}
