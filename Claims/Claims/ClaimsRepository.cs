using Microsoft.EntityFrameworkCore;

namespace Claims.Claims;

public class ClaimsRepository(ClaimsContext claimsContext)
{
	public IQueryable<Claim> GetAll()
	{
		return claimsContext.Claims;
	}

	public Task<Claim?> GetById(string id)
	{
		return claimsContext.Claims.Where(claim => claim.Id == id).SingleOrDefaultAsync();
	}

	public async Task CreateAsync(Claim item)
	{
		claimsContext.Claims.Add(item);
		await claimsContext.SaveChangesAsync();
	}

	public async Task DeleteAsync(string id)
	{
		var claim = await GetById(id);
		if (claim is not null)
		{
			claimsContext.Remove(claim);
			await claimsContext.SaveChangesAsync();
		}
	}
}
