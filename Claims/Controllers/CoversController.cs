using Claims.Auditing;
using Claims.Claims;
using Claims.Claims.DTOs;
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
	public decimal ComputePremiumAsync(NewCoverDTO newCover)
	{
		return PremiumComputer.ComputePremium(newCover);
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
	public async Task<ActionResult> CreateAsync(NewCoverDTO newCover)
	{
		var cover = new Cover(newCover);
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
}
