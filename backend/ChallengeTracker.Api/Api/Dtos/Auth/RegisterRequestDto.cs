using System.ComponentModel.DataAnnotations;

namespace ChallengeTracker.Api.Dtos.Auth;

public record RegisterRequestDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required, MaxLength(64)] string DisplayName
);
