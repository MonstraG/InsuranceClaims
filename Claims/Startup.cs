using Claims.Auditing;
using Claims.Claims.Models;
using Claims.Claims.Repositories;

namespace Claims;

public static class ServiceExtensions
{
	public static void AddMyServices(this IServiceCollection services)
	{
		services.AddSingleton<Auditer>();
		services.AddScoped<Repository<Claim>, ClaimsRepository>();
		services.AddScoped<Repository<Cover>, CoversRepository>();
	}
}
