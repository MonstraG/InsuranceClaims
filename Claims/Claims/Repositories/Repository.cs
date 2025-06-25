using Claims.Claims.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Claims.Repositories;

public abstract class Repository<T>(ClaimsContext claimsContext)
	where T : class, IIdentifiable
{
	private DbSet<T> Set => claimsContext.Set<T>();

	public IQueryable<T> GetAll()
	{
		return Set;
	}

	public Task<T?> GetById(string id)
	{
		return Set.Where(claim => claim.Id == id).SingleOrDefaultAsync();
	}

	public async Task CreateAsync(T item)
	{
		Set.Add(item);
		await claimsContext.SaveChangesAsync();

		OnCreateAsync(item);
	}

	public async Task DeleteAsync(string id)
	{
		var claim = await GetById(id);
		if (claim is not null)
		{
			Set.Remove(claim);
			await claimsContext.SaveChangesAsync();
		}

		OnDeleteAsync(id);
	}

	protected virtual void OnCreateAsync(T item) { }

	protected virtual void OnDeleteAsync(string id) { }
}
