using ChallengeTracker.Api.Dtos.Leaderboards;

namespace ChallengeTracker.Api.Application.Interfaces;

public interface ILeaderboardService
{
  Task<LeaderboardResponseDto?> GetTotalAsync(Guid challengeId, int top = 10);
}
