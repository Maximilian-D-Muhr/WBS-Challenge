namespace ChallengeTracker.Api.Dtos.Auth;

public record AuthResponseDto(string Token, DateTime ExpiresAtUtc);
