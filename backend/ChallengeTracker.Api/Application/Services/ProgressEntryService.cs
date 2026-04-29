using Microsoft.EntityFrameworkCore;

using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.ProgressEntries;
using ChallengeTracker.Api.Infrastructure;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Application.Services;

public class ProgressEntryService : IProgressEntryService
{
  // Owner-only edits/deletes are allowed within this window after creation.
  private static readonly TimeSpan EditWindow = TimeSpan.FromHours(24);

  private readonly ApplicationDbContext _db;

  public ProgressEntryService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<ProgressEntry> LogAsync(Guid userId, CreateProgressEntryDto dto)
  {
    var challenge = await _db.Challenges.FirstOrDefaultAsync(c => c.Id == dto.ChallengeId)
        ?? throw new ArgumentException("Challenge not found.");

    if (challenge.Status != ChallengeStatus.Running)
      throw new ArgumentException("Progress can only be logged on a running challenge.");

    var loggedAt = dto.LoggedAt ?? DateOnly.FromDateTime(DateTime.UtcNow);

    if (loggedAt < challenge.StartDate || loggedAt > challenge.EndDate)
      throw new ArgumentException("LoggedAt must be within the challenge window.");

    var membership = await _db.Memberships
        .FirstOrDefaultAsync(m => m.UserId == userId && m.ChallengeId == dto.ChallengeId)
        ?? throw new ArgumentException("Join the challenge before logging progress.");

    if (membership.Status != MembershipStatus.Active)
      throw new ArgumentException("Only active members can log progress.");

    // The (UserId, ChallengeId, LoggedAt) unique index also guards this — we check first
    // for a clean 409 instead of a DB exception.
    var existing = await _db.ProgressEntries
        .AnyAsync(p => p.UserId == userId
                    && p.ChallengeId == dto.ChallengeId
                    && p.LoggedAt == loggedAt);

    if (existing)
      throw new InvalidOperationException("A progress entry for this day already exists.");

    var entry = new ProgressEntry
    {
      UserId = userId,
      ChallengeId = dto.ChallengeId,
      Amount = dto.Amount,
      Note = dto.Note,
      LoggedAt = loggedAt
    };

    _db.ProgressEntries.Add(entry);
    await _db.SaveChangesAsync();
    return entry;
  }

  public async Task<ProgressEntry?> UpdateAsync(Guid userId, Guid entryId, UpdateProgressEntryDto dto)
  {
    var entry = await _db.ProgressEntries.FirstOrDefaultAsync(p => p.Id == entryId);
    if (entry is null || entry.UserId != userId) return null;

    if (DateTimeOffset.UtcNow - entry.CreatedAt > EditWindow)
      throw new ArgumentException("Edit window has expired.");

    entry.Amount = dto.Amount;
    entry.Note = dto.Note;
    await _db.SaveChangesAsync();
    return entry;
  }

  public async Task<bool> DeleteAsync(Guid userId, Guid entryId)
  {
    var entry = await _db.ProgressEntries.FirstOrDefaultAsync(p => p.Id == entryId);
    if (entry is null || entry.UserId != userId) return false;

    if (DateTimeOffset.UtcNow - entry.CreatedAt > EditWindow)
      throw new ArgumentException("Delete window has expired.");

    _db.ProgressEntries.Remove(entry);
    await _db.SaveChangesAsync();
    return true;
  }

  public async Task<ProgressEntry?> GetMyTodayAsync(Guid userId, Guid challengeId)
  {
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    return await _db.ProgressEntries
        .FirstOrDefaultAsync(p => p.UserId == userId
                               && p.ChallengeId == challengeId
                               && p.LoggedAt == today);
  }

  public async Task<IReadOnlyList<ProgressEntry>> ListMineAsync(Guid userId, Guid challengeId)
  {
    return await _db.ProgressEntries
        .Where(p => p.UserId == userId && p.ChallengeId == challengeId)
        .OrderByDescending(p => p.LoggedAt)
        .ToListAsync();
  }
}
