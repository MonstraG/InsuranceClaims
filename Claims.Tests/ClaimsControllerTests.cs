using System.Net.Http.Json;
using Claims.Claims.DTOs;
using Claims.Claims.Models;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTest
{
	[Fact]
	public async Task Get_Claims()
	{
		using var testAppContext = new TestAppContext();

		var response = await testAppContext.Client.GetAsync(
			"/Claims",
			cancellationToken: TestContext.Current.CancellationToken
		);

		response.EnsureSuccessStatusCode();

		var responseBody = response.Content.ReadFromJsonAsAsyncEnumerable<Claim>(
			cancellationToken: TestContext.Current.CancellationToken
		);

		Assert.Empty(responseBody);
	}

	[Fact]
	public async Task ClaimGetsCreated()
	{
		using var testAppContext = new TestAppContext();

		var coverStart = DateTime.UtcNow;
		var cover = new Cover(
			new NewCoverDTO
			{
				EndDate = DateTime.UtcNow.AddDays(1),
				StartDate = coverStart,
				Type = Cover.Types.Yacht,
			}
		);

		testAppContext.ClaimsContext.Covers.Add(cover);
		await testAppContext.ClaimsContext.SaveChangesAsync(TestContext.Current.CancellationToken);

		var response = await testAppContext.Client.PostAsJsonAsync(
			"/Claims",
			new NewClaimDTO
			{
				CoverId = cover.Id,
				Name = "",
				Type = ClaimType.Collision,
				Created = coverStart.AddMinutes(1),
				DamageCost = 1,
			},
			cancellationToken: TestContext.Current.CancellationToken
		);

		response.EnsureSuccessStatusCode();

		var claim = await response.Content.ReadFromJsonAsync<Claim>(
			cancellationToken: TestContext.Current.CancellationToken,
			options: TestHelpers.DefaultOptions
		);

		Assert.NotNull(claim);
		Assert.NotEmpty(claim.Id);
		Assert.Equal(claim.CoverId, cover.Id);
	}
}
