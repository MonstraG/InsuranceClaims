using Claims.Claims.DTOs;
using Claims.Claims.Models;
using Claims.Claims.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController(
	Repository<Claim> claimsRepository,
	Repository<Cover> coversRepository
) : ControllerBase
{
	// Note to reviewers:
	// Here is a documentation example below.
	// I made these filters up, as illustrative example what could be documented,
	// but I'm not quite sure what to write in other places,
	// as just typing "Gets claim by id" above `GetById` sounds really useless.

	/// <param name="coverId">If set, only show claims for this coverId</param>
	/// <param name="from">If set, only show claims that are created after this date</param>
	/// <param name="to">If set, only show claims that are created before this date</param>
	/// <returns>List of claims matching specified filters</returns>
	[HttpGet]
	public IQueryable<Claim> GetAsync(string? coverId, DateTime? from, DateTime? to)
	{
		var query = claimsRepository.GetAll();
		if (coverId != null)
		{
			query = query.Where(c => c.CoverId == coverId);
		}
		if (from != null)
		{
			query = query.Where(c => c.Created >= from);
		}
		if (to != null)
		{
			query = query.Where(c => c.Created <= to);
		}
		return query;
	}

	[HttpGet("{id}")]
	public Task<Claim?> GetAsync(string id)
	{
		return claimsRepository.GetById(id);
	}

	[HttpPost]
	public async Task<ActionResult<Claim>> CreateAsync(NewClaimDTO newClaim)
	{
		var cover = await coversRepository.GetById(newClaim.CoverId);
		if (cover == null)
		{
			return BadRequest($"cover {newClaim.CoverId} not found");
		}

		if (newClaim.Created <= cover.StartDate)
		{
			return BadRequest(
				$"claim date {newClaim.Created} is too early for cover that starts on {cover.StartDate}"
			);
		}

		if (newClaim.Created >= cover.EndDate)
		{
			return BadRequest(
				$"claim date {newClaim.Created} is too late for cover that ends on {cover.EndDate}"
			);
		}

		var claim = new Claim(newClaim);
		await claimsRepository.CreateAsync(claim);
		return claim;
	}

	[HttpDelete("{id}")]
	public async Task DeleteAsync(string id)
	{
		await claimsRepository.DeleteAsync(id);
	}
}
