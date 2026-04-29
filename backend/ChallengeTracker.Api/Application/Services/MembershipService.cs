using Microsoft.EntityFrameworkCore;

using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Infrastructure;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Application.Services;

public class MembershipService : IMembershipService
{
  private readonly ApplicationDbContext _db;

  public MembershipService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<Membership> JoinAsync(Guid userId, Guid challengeId)
  {
    var challenge = await _db.Challenges.FindAsync(challengeId)
        ?? throw new ArgumentException("Challenge not found");

    var existing = await _db.Memberships
        .FirstOrDefaultAsync(m => m.UserId == userId && m.ChallengeId == challengeId);

    if (existing is not null && existing.Status != MembershipStatus.Left)
      throw new ArgumentException("Already a member of this challenge");

    // Re-joining a challenge after leaving reactivates the previous row instead of creating a new one.
    if (existing is not null)
    {
      existing.Status = challenge.Visibility == ChallengeVisibility.Public
          ? MembershipStatus.Active
          : MembershipStatus.Pending;
      existing.JoinedAt = DateTimeOffset.UtcNow;
      await _db.SaveChangesAsync();
      return existing;
    }

    var membership = new Membership
    {
      UserId = userId,
      ChallengeId = challengeId,
      Status = challenge.Visibility == ChallengeVisibility.Public
          ? MembershipStatus.Active
          : MembershipStatus.Pending
    };

    _db.Memberships.Add(membership);
    await _db.SaveChangesAsync();
    return membership;
  }

  public async Task<bool> LeaveAsync(Guid userId, Guid membershipId)
  {
    var membership = await _db.Memberships.FindAsync(membershipId);
    if (membership is null) return false;
    if (membership.UserId != userId) return false;

    membership.Status = MembershipStatus.Left;
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<Membership?> ApproveAsync(Guid ownerId, Guid membershipId)
  {
    var membership = await _db.Memberships
        .Include(m => m.Challenge)
        .FirstOrDefaultAsync(m => m.Id == membershipId);

    if (membership is null) return null;
    if (membership.Challenge?.OwnerId != ownerId) return null;
    if (membership.Status != MembershipStatus.Pending)
      throw new ArgumentException("Membership is not pending");

    membership.Status = MembershipStatus.Active;
    await _db.SaveChangesAsync();
    return membership;
  }

  public async Task<Membership?> GetAsync(Guid id)
  {
    return await _db.Memberships
        .Include(m => m.Challenge)
        .FirstOrDefaultAsync(m => m.Id == id);
  }

  // Phase 5 — used by GET /memberships/me/{challengeId} to drive the Join/Leave UI.
  public async Task<Membership?> GetMineAsync(Guid userId, Guid challengeId)
  {
    return await _db.Memberships
        .FirstOrDefaultAsync(m => m.UserId == userId && m.ChallengeId == challengeId);
  }
}
