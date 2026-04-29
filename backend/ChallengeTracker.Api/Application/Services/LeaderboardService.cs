using Microsoft.EntityFrameworkCore;

using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.Leaderboards;
using ChallengeTracker.Api.Infrastructure;

namespace ChallengeTracker.Api.Application.Services;

public class LeaderboardService : ILeaderboardService
{
  private readonly ApplicationDbContext _db;

  public LeaderboardService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<LeaderboardResponseDto?> GetTotalAsync(Guid challengeId, int top = 10)
  {
    var challenge = await _db.Challenges.FirstOrDefaultAsync(c => c.Id == challengeId);
    if (challenge is null) return null;

    // Sum amounts per user for this challenge, then join the user displayName.
    var aggregates = await _db.ProgressEntries
        .Where(p => p.ChallengeId == challengeId)
        .GroupBy(p => p.UserId)
        .Select(g => new
        {
          UserId = g.Key,
          TotalAmount = g.Sum(p => p.Amount),
          EntryCount = g.Count()
        })
        .OrderByDescending(x => x.TotalAmount)
        .Take(top)
        .ToListAsync();

    if (aggregates.Count == 0)
    {
      return new LeaderboardResponseDto(challenge.Id, "total", new List<LeaderboardEntryDto>());
    }

    var userIds = aggregates.Select(a => a.UserId).ToList();
    var users = await _db.Users
        .Where(u => userIds.Contains(u.Id))
        .ToDictionaryAsync(u => u.Id, u => u.DisplayName);

    var entries = aggregates
        .Select((a, idx) => new LeaderboardEntryDto(
            Rank: idx + 1,
            UserId: a.UserId,
            DisplayName: users.TryGetValue(a.UserId, out var name) ? name : "(unknown)",
            TotalAmount: a.TotalAmount,
            EntryCount: a.EntryCount))
        .ToList();

    return new LeaderboardResponseDto(challenge.Id, "total", entries);
  }
}
