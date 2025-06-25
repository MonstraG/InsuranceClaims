using Claims.Claims;
using Claims.Claims.DTOs;
using Claims.Claims.Models;
using Xunit;

namespace Claims.Tests.Tests;

public class PremiumComputerTest
{
	[Fact]
	public void IsStable()
	{
		var cover = new NewCoverDTO
		{
			StartDate = DateTime.UtcNow,
			EndDate = DateTime.UtcNow.AddDays(1),
			Type = Cover.Types.Yacht,
		};

		var first = PremiumComputer.ComputePremium(cover);
		var second = PremiumComputer.ComputePremium(cover);
		Assert.Equal(first, second);
	}

	// I relied on my own calculator math for those, and in real world would look for authoritative sources of this
	public static TheoryData<NewCoverDTO, decimal> ExpectedPrices =>
		new()
		{
			{
				new NewCoverDTO
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(1),
					Type = Cover.Types.Yacht,
				},
				1250 * 1.1m
			},
			{
				new NewCoverDTO
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(1),
					Type = Cover.Types.PassengerShip,
				},
				1250 * 1.2m
			},
			{
				new NewCoverDTO
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(2),
					Type = Cover.Types.Yacht,
				},
				1250 * 1.1m * 2
			},
			{
				new NewCoverDTO
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(31),
					Type = Cover.Types.Yacht,
				},
				1250 * 1.1m * 30 + 1375 * 0.95m
			},
			{
				new NewCoverDTO
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(31),
					Type = Cover.Types.Yacht,
				},
				1250 * 1.1m * 30 + 1250 * 1.1m * 0.95m
			},
			{
				new NewCoverDTO
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(181),
					Type = Cover.Types.Yacht,
				},
				1250 * 1.1m * 30 + 1250 * 1.1m * 150 * 0.95m + 1250 * 1.1m * 1 * 0.97m
			},
		};

	[Theory]
	[MemberData(nameof(ExpectedPrices))]
	public void GivesExpectedPrices(NewCoverDTO cover, decimal expectedPremium)
	{
		// I actually wonder what would be the tolerance here in the real world
		const int tolerance = 4;

		Assert.Equal(PremiumComputer.ComputePremium(cover), expectedPremium, tolerance);
	}

	[Fact]
	public void ThrowsForInvalidCovers()
	{
		Assert.Throws<ArgumentOutOfRangeException>(() =>
			PremiumComputer.ComputePremium(
				new NewCoverDTO
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(PremiumComputer.MaxCoverDays + 1),
					Type = Cover.Types.Yacht,
				}
			)
		);

		Assert.Throws<ArgumentOutOfRangeException>(() =>
			PremiumComputer.ComputePremium(
				new NewCoverDTO
				{
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow.AddDays(-1),
					Type = Cover.Types.Yacht,
				}
			)
		);
	}
}
