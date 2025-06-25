using Claims.Auditing;
using Claims.Claims.Models;

namespace Claims.Claims.Repositories;

public class ClaimsRepository(ClaimsContext claimsContext, Auditer auditer)
	: Repository<Claim>(claimsContext)
{
	protected override Task OnCreateAsync(Claim item) => auditer.AuditClaim(item.Id, "POST");

	protected override Task OnDeleteAsync(string id) => auditer.AuditClaim(id, "DELETE");
}
