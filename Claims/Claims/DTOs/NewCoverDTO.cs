using Claims.Claims.Models;

namespace Claims.Claims.DTOs;

public record NewCoverDTO : ICover
{
	public required DateTime StartDate { get; init; }
	public required DateTime EndDate { get; init; }
	public required Cover.Types Type { get; init; }
}
