using Claims.Auditing;
using Claims.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController(
	ILogger<ClaimsController> logger,
	ClaimsRepository claimsRepository,
	Auditer auditer
) : ControllerBase
{
	[HttpGet]
	public IQueryable<Claim> GetAsync()
	{
		return claimsRepository.GetAll();
	}

	[HttpPost]
	public async Task<Claim> CreateAsync(Claim claim)
	{
		claim.Id = Guid.NewGuid().ToString();
		await claimsRepository.CreateAsync(claim);
		auditer.AuditClaim(claim.Id, "POST");
		return claim;
	}

	[HttpDelete("{id}")]
	public async Task DeleteAsync(string id)
	{
		auditer.AuditClaim(id, "DELETE");
		await claimsRepository.DeleteAsync(id);
	}

	[HttpGet("{id}")]
	public Task<Claim?> GetAsync(string id)
	{
		return claimsRepository.GetById(id);
	}
}
