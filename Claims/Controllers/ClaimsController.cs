using Claims.Claims.DTOs;
using Claims.Claims.Models;
using Claims.Claims.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController(Repository<Claim> claimsRepository) : ControllerBase
{
	[HttpGet]
	public IQueryable<Claim> GetAsync()
	{
		return claimsRepository.GetAll();
	}

	[HttpPost]
	public async Task<Claim> CreateAsync(NewClaimDTO newClaim)
	{
		var claim = new Claim(newClaim);
		await claimsRepository.CreateAsync(claim);
		return claim;
	}

	[HttpDelete("{id}")]
	public async Task DeleteAsync(string id)
	{
		await claimsRepository.DeleteAsync(id);
	}

	[HttpGet("{id}")]
	public Task<Claim?> GetAsync(string id)
	{
		return claimsRepository.GetById(id);
	}
}
