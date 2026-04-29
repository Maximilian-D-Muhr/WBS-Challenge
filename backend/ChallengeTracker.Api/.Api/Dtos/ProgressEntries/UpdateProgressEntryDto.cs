using System.ComponentModel.DataAnnotations;

namespace ChallengeTracker.Api.Dtos.ProgressEntries;

public record UpdateProgressEntryDto(
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    decimal Amount,
    [MaxLength(500)] string? Note
);
