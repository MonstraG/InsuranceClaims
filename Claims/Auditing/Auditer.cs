using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Claims.Auditing;

public class Auditer(IDbContextFactory<AuditContext> auditContextFactory)
{
	public void AuditClaim(
		string id,
		[AllowedValues("POST", "PUT", "DELETE")] string httpRequestType
	)
	{
		Task.Run(async () =>
		{
			await using var auditContext = await auditContextFactory.CreateDbContextAsync();

			var claimAudit = new ClaimAudit
			{
				Created = DateTime.Now,
				HttpRequestType = httpRequestType,
				ClaimId = id,
			};

			auditContext.Add(claimAudit);
			await auditContext.SaveChangesAsync();
		});
	}

	public void AuditCover(
		string id,
		[AllowedValues("POST", "PUT", "DELETE")] string httpRequestType
	)
	{
		Task.Run(async () =>
		{
			await using var auditContext = await auditContextFactory.CreateDbContextAsync();

			var coverAudit = new CoverAudit
			{
				Created = DateTime.Now,
				HttpRequestType = httpRequestType,
				CoverId = id,
			};

			auditContext.Add(coverAudit);
			await auditContext.SaveChangesAsync();
		});
	}
}
