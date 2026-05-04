using System.ComponentModel.DataAnnotations;

namespace ChallengeTracker.Api.Api.Filters;

public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
  public async ValueTask<object?> InvokeAsync(
      EndpointFilterInvocationContext context,
      EndpointFilterDelegate next)
  {
    var dto = context.Arguments.OfType<T>().FirstOrDefault();
    if (dto is null)
      return Results.BadRequest(new
      {
        error = $"Request body must include a {typeof(T).Name}"
      });

    var results = new List<ValidationResult>();
    var vc = new ValidationContext(dto);
    var isValid = Validator.TryValidateObject(dto, vc, results, validateAllProperties: true);

    if (!isValid)
    {
      var errors = results
          .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
          .ToDictionary(
              g => g.Key,
              g => g.Select(r => r.ErrorMessage ?? "Invalid").ToArray());

      return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
    }

    return await next(context);
  }
}
