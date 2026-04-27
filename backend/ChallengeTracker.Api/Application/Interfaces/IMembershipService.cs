using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Application.Interfaces;

public interface IMembershipService
{
  Task<Membership> JoinAsync(Guid userId, Guid challengeId);
  Task<bool> LeaveAsync(Guid userId, Guid membershipId);
  Task<Membership?> ApproveAsync(Guid ownerId, Guid membershipId);
  Task<Membership?> GetAsync(Guid id);
}
