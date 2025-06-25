using Claims.Auditing;
using Claims.Claims;
using Claims.Claims.Models;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(
	ILogger<CoversController> logger,
	Repository<Cover> coversRepository,
	Auditer auditer
) : ControllerBase
{
	[HttpPost("compute")]
	public decimal ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
	{
		return ComputePremium(startDate, endDate, coverType);
	}

	[HttpGet]
	public IQueryable<Cover> GetAsync()
	{
		return coversRepository.GetAll();
	}

	[HttpGet("{id}")]
	public Task<Cover?> GetAsync(string id)
	{
		return coversRepository.GetById(id);
	}

	[HttpPost]
	public async Task<ActionResult> CreateAsync(Cover cover)
	{
		cover.Id = Guid.NewGuid().ToString();
		cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
		await coversRepository.CreateAsync(cover);
		auditer.AuditCover(cover.Id, "POST");
		return Ok(cover);
	}

	[HttpDelete("{id}")]
	public async Task DeleteAsync(string id)
	{
		auditer.AuditCover(id, "DELETE");
		await coversRepository.DeleteAsync(id);
	}

	private static decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
	{
		var multiplier = 1.3m;
		if (coverType == CoverType.Yacht)
		{
			multiplier = 1.1m;
		}

		if (coverType == CoverType.PassengerShip)
		{
			multiplier = 1.2m;
		}

		if (coverType == CoverType.Tanker)
		{
			multiplier = 1.5m;
		}

		var premiumPerDay = 1250 * multiplier;
		var insuranceLength = (endDate - startDate).TotalDays;
		var totalPremium = 0m;

		for (var i = 0; i < insuranceLength; i++)
		{
			if (i < 30)
				totalPremium += premiumPerDay;
			if (i < 180 && coverType == CoverType.Yacht)
				totalPremium += premiumPerDay - premiumPerDay * 0.05m;
			else if (i < 180)
				totalPremium += premiumPerDay - premiumPerDay * 0.02m;
			if (i < 365 && coverType != CoverType.Yacht)
				totalPremium += premiumPerDay - premiumPerDay * 0.03m;
			else if (i < 365)
				totalPremium += premiumPerDay - premiumPerDay * 0.08m;
		}

		return totalPremium;
	}
}
