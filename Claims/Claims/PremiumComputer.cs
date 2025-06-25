using Claims.Claims.Models;

namespace Claims.Claims;

public class PremiumComputer
{
	public const int MaxCoverDays = 365;

	private const decimal BaseDayRate = 1250;

	private const int Month = 30;
	private const int HalfYear = 180;

	// For this logic (as desired), I can see 2 interpretations
	// 1) discount is multiplicative with price at the end
	// 2) discount percentage is subtracted from the cover type multiplier
	// I assume it is first.
	public static decimal ComputePremium(ICover cover)
	{
		var insuranceLength = GetInsuranceLength(cover);
		if (insuranceLength > MaxCoverDays)
		{
			throw new ArgumentOutOfRangeException(nameof(cover), "Cover duration too long");
		}

		var firstMonth = Math.Max(insuranceLength, Month);
		var halfYear = Math.Max(insuranceLength - firstMonth, HalfYear - Month);
		var remainder = Math.Max(insuranceLength - firstMonth - halfYear, MaxCoverDays);

		return GetPrice(firstMonth, cover.Type, GetFirstMonthDiscount(cover.Type))
			+ GetPrice(insuranceLength, cover.Type, GetHalfYearDiscount(cover.Type))
			+ GetPrice(remainder, cover.Type, GetRemainderDiscount(cover.Type));
	}

	private static decimal GetPrice(double period, Cover.Types coverType, decimal durationDiscount)
	{
		var coverTypeMultiplier = GetCoverTypeMultiplier(coverType);
		return BaseDayRate * (decimal)period * coverTypeMultiplier * durationDiscount;
	}

	private static decimal GetCoverTypeMultiplier(Cover.Types coverType)
	{
		return coverType switch
		{
			Cover.Types.Yacht => 1.1m,
			Cover.Types.PassengerShip => 1.2m,
			Cover.Types.Tanker => 1.5m,
			_ => 1.3m,
		};
	}

	private static decimal GetFirstMonthDiscount(Cover.Types _)
	{
		return 1;
	}

	private static decimal GetHalfYearDiscount(Cover.Types coverType)
	{
		return coverType switch
		{
			Cover.Types.Yacht => 1 - 0.05m,
			_ => 1 - 0.02m,
		};
	}

	private static decimal GetRemainderDiscount(Cover.Types coverType)
	{
		return coverType switch
		{
			Cover.Types.Yacht => 1 - 0.03m,
			_ => 1 - 0.01m,
		};
	}

	// In real world I will ask about leap years.
	public static double GetInsuranceLength(ICover cover)
	{
		return (cover.EndDate - cover.StartDate).TotalDays;
	}
}

public interface ICover
{
	public DateTime StartDate { get; }
	public DateTime EndDate { get; }
	public Cover.Types Type { get; }
}
