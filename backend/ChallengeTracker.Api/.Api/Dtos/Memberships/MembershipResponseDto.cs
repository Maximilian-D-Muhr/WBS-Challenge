using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Dtos.Memberships;

public record MembershipResponseDto(
    Guid Id,
    Guid UserId,
    Guid ChallengeId,
    MembershipStatus Status,
    DateTimeOffset JoinedAt
);
