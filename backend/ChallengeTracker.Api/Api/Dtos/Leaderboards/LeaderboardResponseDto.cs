namespace ChallengeTracker.Api.Dtos.Leaderboards;

public record LeaderboardResponseDto(
    Guid ChallengeId,
    string Period,
    IReadOnlyList<LeaderboardEntryDto> Entries
);
