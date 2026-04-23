using Microsoft.AspNetCore.Identity;

namespace ChallengeTracker.Api.Models;

public class User : IdentityUser<Guid>
{
  public string DisplayName { get; set; } = string.Empty;
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  // Navigation
  public ICollection<Challenge> OwnedChallenges { get; set; } = new List<Challenge>();
  public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
  public ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
}
