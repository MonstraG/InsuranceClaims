using Claims.Claims;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Tests;

public class TestAppContext : IDisposable
{
	private readonly WebApplicationFactory<TestApp> _factory;
	private readonly IServiceScope _scope;

	public ClaimsContext ClaimsContext { get; }
	public HttpClient Client { get; }

	public TestAppContext()
	{
		_factory = new WebApplicationFactory<TestApp>().WithWebHostBuilder(_ => { });
		Client = _factory.CreateClient();
		_scope = _factory.Services.CreateScope();
		ClaimsContext = _scope.ServiceProvider.GetRequiredService<ClaimsContext>();
	}

	public void Dispose()
	{
		Client.Dispose();
		_scope.Dispose();
		_factory.Dispose();
	}
}
