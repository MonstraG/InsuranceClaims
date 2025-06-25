using System.ComponentModel.DataAnnotations;
using Claims.Claims.Models;

namespace Claims.Claims.DTOs;

public record NewClaimDTO
{
	public required string CoverId { get; init; }
	public required string Name { get; init; }
	public required ClaimType Type { get; init; }

	// From the requirements, it seems that this:
	// a) settable (and not just DateTime.UtcNow)
	// b) settable in the past
	// I'll allow it.
	// In the real world I would verify things like that
	public required DateTime Created { get; init; }

	[Range(1, 100_000)]
	public required decimal DamageCost { get; init; }
}
