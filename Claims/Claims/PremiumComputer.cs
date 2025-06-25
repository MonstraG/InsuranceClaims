using Claims.Claims.Models;

namespace Claims.Claims;

public class PremiumComputer
{
	public static decimal ComputePremium(ICover newCover)
	{
		var multiplier = 1.3m;
		if (newCover.Type == Cover.Types.Yacht)
		{
			multiplier = 1.1m;
		}

		if (newCover.Type == Cover.Types.PassengerShip)
		{
			multiplier = 1.2m;
		}

		if (newCover.Type == Cover.Types.Tanker)
		{
			multiplier = 1.5m;
		}

		var premiumPerDay = 1250 * multiplier;
		var insuranceLength = (newCover.EndDate - newCover.StartDate).TotalDays;
		var totalPremium = 0m;

		for (var i = 0; i < insuranceLength; i++)
		{
			if (i < 30)
				totalPremium += premiumPerDay;
			if (i < 180 && newCover.Type == Cover.Types.Yacht)
				totalPremium += premiumPerDay - premiumPerDay * 0.05m;
			else if (i < 180)
				totalPremium += premiumPerDay - premiumPerDay * 0.02m;
			if (i < 365 && newCover.Type != Cover.Types.Yacht)
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
}
