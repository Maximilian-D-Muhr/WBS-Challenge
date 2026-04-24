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

    return await query
        .OrderByDescending(c => c.CreatedAt)
        .ToListAsync();
  }

  public async Task<Challenge?> GetAsync(Guid id)
  {
    return await _db.Challenges
        .Include(c => c.Owner)
        .Include(c => c.Memberships)
        .FirstOrDefaultAsync(c => c.Id == id);
  }
}
