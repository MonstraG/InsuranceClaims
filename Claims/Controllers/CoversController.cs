using Claims.Claims;
using Claims.Claims.DTOs;
using Claims.Claims.Models;
using Claims.Claims.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(Repository<Cover> coversRepository) : ControllerBase
{
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
	public async Task<ActionResult<Cover>> CreateAsync(NewCoverDTO newCover)
	{
		if (newCover.StartDate < DateTime.UtcNow)
		{
			return BadRequest("cover start date cannot be in the past");
		}

		if (newCover.EndDate <= newCover.StartDate)
		{
			return BadRequest("cover end date cannot be earlier or equal to start date");
		}

		var duration = PremiumComputer.GetInsuranceLength(newCover);
		if (duration > PremiumComputer.MaxCoverDays)
		{
			return BadRequest(
				$"total insurance period cannot exceed 1 year, actual length = {duration:F} days"
			);
		}

		var cover = new Cover(newCover);
		await coversRepository.CreateAsync(cover);
		return cover;
	}

	[HttpDelete("{id}")]
	public async Task DeleteAsync(string id)
	{
		await coversRepository.DeleteAsync(id);
	}

	[HttpPost("compute")]
	public decimal ComputePremiumAsync(NewCoverDTO newCover)
	{
		return PremiumComputer.ComputePremium(newCover);
	}
}
