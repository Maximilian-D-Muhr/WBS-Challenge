using ChallengeTracker.Api.Dtos.ProgressEntries;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Application.Interfaces;

public interface IProgressEntryService
{
  Task<ProgressEntry> LogAsync(Guid userId, CreateProgressEntryDto dto);
  Task<ProgressEntry?> UpdateAsync(Guid userId, Guid entryId, UpdateProgressEntryDto dto);
  Task<bool> DeleteAsync(Guid userId, Guid entryId);
  Task<ProgressEntry?> GetMyTodayAsync(Guid userId, Guid challengeId);
  Task<IReadOnlyList<ProgressEntry>> ListMineAsync(Guid userId, Guid challengeId);
}
