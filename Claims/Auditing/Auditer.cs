namespace Claims.Auditing;

public class Auditer(AuditContext auditContext)
{
	public void AuditClaim(string id, string httpRequestType)
	{
		var claimAudit = new ClaimAudit
		{
			Created = DateTime.Now,
			HttpRequestType = httpRequestType,
			ClaimId = id,
		};

		auditContext.Add(claimAudit);
		auditContext.SaveChanges();
	}

	public void AuditCover(string id, string httpRequestType)
	{
		var coverAudit = new CoverAudit
		{
			Created = DateTime.Now,
			HttpRequestType = httpRequestType,
			CoverId = id,
		};

		auditContext.Add(coverAudit);
		auditContext.SaveChanges();
	}
}
