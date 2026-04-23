using System.ComponentModel.DataAnnotations;

namespace ChallengeTracker.Api.Dtos.Auth;

public record RegisterRequestDto(
    [property: Required]
    [property: EmailAddress]
    string Email,

    [property: Required]
    [property: StringLength(100, MinimumLength = 8)]
    string Password,

    [property: Required]
    [property: StringLength(50, MinimumLength = 1)]
    string DisplayName
);
