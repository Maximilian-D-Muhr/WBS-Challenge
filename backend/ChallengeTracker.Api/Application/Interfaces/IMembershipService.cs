using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Application.Interfaces;

public interface IMembershipService
{
  Task<Membership> JoinAsync(Guid userId, Guid challengeId);
  Task<bool> LeaveAsync(Guid userId, Guid membershipId);
  Task<Membership?> ApproveAsync(Guid ownerId, Guid membershipId);
  Task<Membership?> GetAsync(Guid id);

  // Phase 5 — frontend reads this to render Join vs. Leave on the detail page.
  Task<Membership?> GetMineAsync(Guid userId, Guid challengeId);
}
