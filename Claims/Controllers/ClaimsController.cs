using Claims.Auditing;
using Claims.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController(
	ILogger<ClaimsController> logger,
	ClaimsContext claimsContext,
	AuditContext auditContext
) : ControllerBase
{
	private readonly Auditer _auditer = new(auditContext);

	[HttpGet]
	public async Task<IEnumerable<Claim>> GetAsync()
	{
		return await claimsContext.GetClaimsAsync();
	}

	[HttpPost]
	public async Task<ActionResult> CreateAsync(Claim claim)
	{
		claim.Id = Guid.NewGuid().ToString();
		await claimsContext.AddItemAsync(claim);
		_auditer.AuditClaim(claim.Id, "POST");
		return Ok(claim);
	}

	[HttpDelete("{id}")]
	public async Task DeleteAsync(string id)
	{
		_auditer.AuditClaim(id, "DELETE");
		await claimsContext.DeleteItemAsync(id);
	}

	[HttpGet("{id}")]
	public async Task<Claim?> GetAsync(string id)
	{
		return await claimsContext.GetClaimAsync(id);
	}
}
