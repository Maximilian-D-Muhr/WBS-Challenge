using System.ComponentModel.DataAnnotations;

using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Dtos.Challenges;

public record CreateChallengeDto(
    [property: Required]
    [property: StringLength(100, MinimumLength = 1)]
    string Title,

    [property: StringLength(1000)]
    string? Description,

    [property: Required]
    ChallengeVisibility Visibility,

    [property: Required]
    DateOnly StartDate,

    [property: Required]
    DateOnly EndDate,

    [property: Required]
    [property: StringLength(50, MinimumLength = 1)]
    string UnitLabel
);
