namespace ChallengeTracker.Api.Dtos.ProgressEntries;

public record ProgressEntryResponseDto(
    Guid Id,
    Guid UserId,
    Guid ChallengeId,
    decimal Amount,
    string? Note,
    DateOnly LoggedAt,
    DateTimeOffset CreatedAt
);
