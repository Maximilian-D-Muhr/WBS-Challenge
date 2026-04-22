namespace ChallengeTracker.Api.Models;

public class Challenge
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid OwnerId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public ChallengeVisibility Visibility { get; set; } = ChallengeVisibility.Public;
  public ChallengeStatus Status { get; set; } = ChallengeStatus.Open;
  public DateOnly StartDate { get; set; }
  public DateOnly EndDate { get; set; }
  public string UnitLabel { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  // Navigation
  public User? Owner { get; set; }
  public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
  public ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
}
