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
	[HttpGet]
	public IQueryable<Claim> GetAsync()
	{
		return claimsRepository.GetAll();
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
