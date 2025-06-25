using Claims.Auditing;
using Claims.Claims.Models;

namespace Claims.Claims.Repositories;

public class CoversRepository(ClaimsContext claimsContext, Auditer auditer)
	: Repository<Cover>(claimsContext)
{
	protected override void OnCreateAsync(Cover item) => auditer.AuditCover(item.Id, "POST");

	protected override void OnDeleteAsync(string id) => auditer.AuditCover(id, "DELETE");
}
