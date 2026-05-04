using System.ComponentModel.DataAnnotations;

namespace ChallengeTracker.Api.Dtos.Auth;

public record LoginRequestDto(
    [Required, EmailAddress] string Email,
    [Required] string Password
);
