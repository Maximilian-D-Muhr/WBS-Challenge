using Microsoft.EntityFrameworkCore;

using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.Challenges;
using ChallengeTracker.Api.Infrastructure;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Application.Services;

public class ChallengeService : IChallengeService
{
  private readonly ApplicationDbContext _db;

  public ChallengeService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<Challenge> CreateAsync(Guid ownerId, CreateChallengeDto dto)
  {
    var challenge = new Challenge
    {
      OwnerId = ownerId,
      Title = dto.Title,
      Description = dto.Description ?? string.Empty,
      Visibility = dto.Visibility,
      Status = ChallengeStatus.Open,
      StartDate = dto.StartDate,
      EndDate = dto.EndDate,
      UnitLabel = dto.UnitLabel
    };

    _db.Challenges.Add(challenge);
    await _db.SaveChangesAsync();
    return challenge;
  }

  public async Task<IReadOnlyList<Challenge>> ListAsync(
      ChallengeVisibility? visibility,
      ChallengeStatus? status)
  {
    var query = _db.Challenges
        .Include(c => c.Owner)
        .AsQueryable();

    if (visibility.HasValue)
      query = query.Where(c => c.Visibility == visibility.Value);

    if (status.HasValue)
      query = query.Where(c => c.Status == status.Value);

    // SQLite can't ORDER BY DateTimeOffset — materialize then sort client-side.
    var list = await query.ToListAsync();
    return list.OrderByDescending(c => c.CreatedAt).ToList();
  }

  public async Task<Challenge?> GetAsync(Guid id)
  {
    return await _db.Challenges
        .Include(c => c.Owner)
        .Include(c => c.Memberships)
        .FirstOrDefaultAsync(c => c.Id == id);
  }

  public async Task<Challenge?> StartAsync(Guid ownerId, Guid challengeId)
  {
    var challenge = await _db.Challenges.FindAsync(challengeId);
    if (challenge is null) return null;
    if (challenge.OwnerId != ownerId) return null;

    if (challenge.Status != ChallengeStatus.Open)
      throw new ArgumentException("Only open challenges can be started");

    challenge.Status = ChallengeStatus.Running;
    await _db.SaveChangesAsync();
    return challenge;
  }

  public async Task<Challenge?> CompleteAsync(Guid ownerId, Guid challengeId)
  {
    var challenge = await _db.Challenges.FindAsync(challengeId);
    if (challenge is null) return null;
    if (challenge.OwnerId != ownerId) return null;

    if (challenge.Status != ChallengeStatus.Running)
      throw new ArgumentException("Only running challenges can be completed");

    challenge.Status = ChallengeStatus.Completed;
    await _db.SaveChangesAsync();
    return challenge;
  }
}
