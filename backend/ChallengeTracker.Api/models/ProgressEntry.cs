namespace ChallengeTracker.Api.Models;

public class ProgressEntry
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid UserId { get; set; }
  public Guid ChallengeId { get; set; }
  public decimal Amount { get; set; }
  public string? Note { get; set; }
  public DateOnly LoggedAt { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  // Navigation
  public User? User { get; set; }
  public Challenge? Challenge { get; set; }
}
