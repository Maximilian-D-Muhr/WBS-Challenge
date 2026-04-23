using System.ComponentModel.DataAnnotations;

namespace ChallengeTracker.Api.Dtos.Auth;

public record LoginRequestDto(
    [property: Required]
    [property: EmailAddress]
    string Email,

    [property: Required]
    string Password
);
