using System.Net.Http.Json;
using Claims.Claims.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTest
{
	[Fact]
	public async Task Get_Claims()
	{
		var application = new WebApplicationFactory<TestApp>().WithWebHostBuilder(_ => { });
		var client = application.CreateClient();

		var response = await client.GetAsync(
			"/Claims",
			cancellationToken: TestContext.Current.CancellationToken
		);

		response.EnsureSuccessStatusCode();

		var responseBody = response.Content.ReadFromJsonAsAsyncEnumerable<Claim>(
			cancellationToken: TestContext.Current.CancellationToken
		);

		Assert.Empty(responseBody);
	}
}
