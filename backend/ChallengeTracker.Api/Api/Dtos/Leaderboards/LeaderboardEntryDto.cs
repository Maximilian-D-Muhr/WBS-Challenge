namespace ChallengeTracker.Api.Dtos.Leaderboards;

public record LeaderboardEntryDto(
    int Rank,
    Guid UserId,
    string DisplayName,
    decimal TotalAmount,
    int EntryCount
);
