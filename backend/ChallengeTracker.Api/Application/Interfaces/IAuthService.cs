using ChallengeTracker.Api.Dtos.Auth;

namespace ChallengeTracker.Api.Application.Interfaces;

public interface IAuthService
{
  Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDto request);
  Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
  Task<MeResponseDto?> GetCurrentUserAsync(Guid userId);
}
