using Claims.Claims.Models;

namespace Claims.Claims;

public class PremiumComputer
{
	public static decimal ComputePremium(ICover cover)
	{
		var multiplier = 1.3m;
		if (cover.Type == Cover.Types.Yacht)
		{
			multiplier = 1.1m;
		}

		if (cover.Type == Cover.Types.PassengerShip)
		{
			multiplier = 1.2m;
		}

		if (cover.Type == Cover.Types.Tanker)
		{
			multiplier = 1.5m;
		}

		var premiumPerDay = 1250 * multiplier;
		var insuranceLength = ICover.GetInsuranceLength(cover);
		var totalPremium = 0m;

		for (var i = 0; i < insuranceLength; i++)
		{
			if (i < 30)
				totalPremium += premiumPerDay;
			if (i < 180 && cover.Type == Cover.Types.Yacht)
				totalPremium += premiumPerDay - premiumPerDay * 0.05m;
			else if (i < 180)
				totalPremium += premiumPerDay - premiumPerDay * 0.02m;
			if (i < 365 && cover.Type != Cover.Types.Yacht)
				totalPremium += premiumPerDay - premiumPerDay * 0.03m;
			else if (i < 365)
				totalPremium += premiumPerDay - premiumPerDay * 0.08m;
		}

		return totalPremium;
	}
}

public interface ICover
{
	public DateTime StartDate { get; }
	public DateTime EndDate { get; }
	public Cover.Types Type { get; }

	// In real world I will ask about leap years.
	// I'm kind of unhappy about this static method. Ideas?
	public static double GetInsuranceLength(ICover cover)
	{
		return (cover.EndDate - cover.StartDate).TotalDays;
	}
}
