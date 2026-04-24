using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Dtos.Challenges;

public record ChallengeResponseDto(
    Guid Id,
    Guid OwnerId,
    string OwnerDisplayName,
    string Title,
    string Description,
    ChallengeVisibility Visibility,
    ChallengeStatus Status,
    DateOnly StartDate,
    DateOnly EndDate,
    string UnitLabel,
    DateTimeOffset CreatedAt
);
