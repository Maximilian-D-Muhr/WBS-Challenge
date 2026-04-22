namespace ChallengeTracker.Api.Models;

public class Membership
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid UserId { get; set; }
  public Guid ChallengeId { get; set; }
  public MembershipStatus Status { get; set; } = MembershipStatus.Pending;
  public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;

  // Navigation
  public User? User { get; set; }
  public Challenge? Challenge { get; set; }
}
