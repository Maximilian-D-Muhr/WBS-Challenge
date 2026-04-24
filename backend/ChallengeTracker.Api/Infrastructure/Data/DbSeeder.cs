using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Infrastructure.Data;

public class DbSeeder
{
  private readonly ApplicationDbContext _db;
  private readonly UserManager<User> _userManager;

  public DbSeeder(ApplicationDbContext db, UserManager<User> userManager)
  {
    _db = db;
    _userManager = userManager;
  }

  public async Task SeedAsync(CancellationToken ct = default)
  {
    // Don't re-seed if a challenge already exists.
    if (await _db.Challenges.AnyAsync(ct)) return;

    var alice = await EnsureUserAsync("alice@example.com", "Alice");
    var bob = await EnsureUserAsync("bob@example.com", "Bob");

    var challenge = new Challenge
    {
      OwnerId = alice.Id,
      Title = "Read 100 pages this week",
      Description = "A kick-off reading streak to try out the app.",
      Visibility = ChallengeVisibility.Public,
      Status = ChallengeStatus.Open,
      StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
      EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
      UnitLabel = "pages"
    };
    _db.Challenges.Add(challenge);

    // Bob joins Alice's public challenge
    _db.Memberships.Add(new Membership
    {
      UserId = bob.Id,
      ChallengeId = challenge.Id,
      Status = MembershipStatus.Active
    });

    await _db.SaveChangesAsync(ct);
  }

  private async Task<User> EnsureUserAsync(string email, string displayName)
  {
    var existing = await _userManager.FindByEmailAsync(email);
    if (existing is not null) return existing;

    var user = new User
    {
      UserName = email,
      Email = email,
      DisplayName = displayName
    };
    await _userManager.CreateAsync(user, "Password1!");
    return user;
  }
}
