using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(
	ClaimsContext claimsContext,
	AuditContext auditContext,
	ILogger<CoversController> logger
) : ControllerBase
{
	private readonly ILogger<CoversController> _logger = logger;
	private readonly Auditer _auditer = new(auditContext);

	[HttpPost("compute")]
	public ActionResult ComputePremiumAsync(
		DateTime startDate,
		DateTime endDate,
		CoverType coverType
	)
	{
		return Ok(ComputePremium(startDate, endDate, coverType));
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
	{
		var results = await claimsContext.Covers.ToListAsync();
		return Ok(results);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Cover>> GetAsync(string id)
	{
		var results = await claimsContext.Covers.ToListAsync();
		return Ok(results.SingleOrDefault(cover => cover.Id == id));
	}

	[HttpPost]
	public async Task<ActionResult> CreateAsync(Cover cover)
	{
		cover.Id = Guid.NewGuid().ToString();
		cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
		claimsContext.Covers.Add(cover);
		await claimsContext.SaveChangesAsync();
		_auditer.AuditCover(cover.Id, "POST");
		return Ok(cover);
	}

	[HttpDelete("{id}")]
	public async Task DeleteAsync(string id)
	{
		_auditer.AuditCover(id, "DELETE");
		var cover = await claimsContext
			.Covers.Where(cover => cover.Id == id)
			.SingleOrDefaultAsync();
		if (cover is not null)
		{
			claimsContext.Covers.Remove(cover);
			await claimsContext.SaveChangesAsync();
		}
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
