using System.ComponentModel.DataAnnotations;

namespace ChallengeTracker.Api.Dtos.Memberships;

public record CreateMembershipDto(
    [property: Required]
    Guid ChallengeId
);
