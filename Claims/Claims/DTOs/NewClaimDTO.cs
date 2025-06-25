using Claims.Claims.Models;

namespace Claims.Claims.DTOs;

public record NewClaimDTO
{
	public required string CoverId { get; init; }
	public required string Name { get; init; }
	public required ClaimType Type { get; init; }
	public required decimal DamageCost { get; init; }
}
