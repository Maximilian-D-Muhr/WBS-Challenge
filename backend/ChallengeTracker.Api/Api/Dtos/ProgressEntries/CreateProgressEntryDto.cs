using System.ComponentModel.DataAnnotations;

namespace ChallengeTracker.Api.Dtos.ProgressEntries;

public record CreateProgressEntryDto(
    [Required] Guid ChallengeId,
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    decimal Amount,
    [MaxLength(500)] string? Note,
    DateOnly? LoggedAt
);
