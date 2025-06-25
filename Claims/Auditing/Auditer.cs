using System.ComponentModel.DataAnnotations;

namespace Claims.Auditing;

public class Auditer(AuditContext auditContext)
{
	public Task AuditClaim(
		string id,
		[AllowedValues("POST", "PUT", "DELETE")] string httpRequestType
	)
	{
		var claimAudit = new ClaimAudit
		{
			Created = DateTime.Now,
			HttpRequestType = httpRequestType,
			ClaimId = id,
		};

		auditContext.Add(claimAudit);
		return auditContext.SaveChangesAsync();
	}

	public Task AuditCover(
		string id,
		[AllowedValues("POST", "PUT", "DELETE")] string httpRequestType
	)
	{
		var coverAudit = new CoverAudit
		{
			Created = DateTime.Now,
			HttpRequestType = httpRequestType,
			CoverId = id,
		};

		auditContext.Add(coverAudit);
		return auditContext.SaveChangesAsync();
	}
}
