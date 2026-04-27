using ChallengeTracker.Api.Dtos.Challenges;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Application.Interfaces;

public interface IChallengeService
{
  Task<Challenge> CreateAsync(Guid ownerId, CreateChallengeDto dto);
  Task<IReadOnlyList<Challenge>> ListAsync(ChallengeVisibility? visibility, ChallengeStatus? status);
  Task<Challenge?> GetAsync(Guid id);
  Task<Challenge?> StartAsync(Guid ownerId, Guid challengeId);
  Task<Challenge?> CompleteAsync(Guid ownerId, Guid challengeId);
}
