using System.Net.Http.Json;
using Claims.Claims.DTOs;
using Claims.Claims.Models;
using Claims.Tests.Setup;
using Xunit;

namespace Claims.Tests.Tests;

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

	[Fact]
	public async Task RefusesClaimsForNonExistentCovers()
	{
		using var testAppContext = new TestAppContext();

		var response = await testAppContext.Client.PostAsJsonAsync(
			"/Claims",
			new NewClaimDTO
			{
				CoverId = "does not exist",
				Name = "",
				Type = ClaimType.Collision,
				Created = DateTime.UtcNow,
				DamageCost = 1,
			},
			cancellationToken: TestContext.Current.CancellationToken
		);

		Assert.False(response.IsSuccessStatusCode);
	}

	[Fact]
	public async Task RefusesClaimsWithWrongDate()
	{
		using var testAppContext = new TestAppContext();

		var coverStart = DateTime.UtcNow;
		var coverEnd = DateTime.UtcNow.AddDays(1);
		var cover = new Cover(
			new NewCoverDTO
			{
				EndDate = coverEnd,
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
				Created = coverStart.AddMicroseconds(1), // correct date
				DamageCost = 1,
			},
			cancellationToken: TestContext.Current.CancellationToken
		);

		// doesn't fail for other reasons
		response.EnsureSuccessStatusCode();

		response = await testAppContext.Client.PostAsJsonAsync(
			"/Claims",
			new NewClaimDTO
			{
				CoverId = cover.Id,
				Name = "",
				Type = ClaimType.Collision,
				Created = coverStart.AddDays(-1), // before started
				DamageCost = 1,
			},
			cancellationToken: TestContext.Current.CancellationToken
		);

		Assert.False(response.IsSuccessStatusCode);

		response = await testAppContext.Client.PostAsJsonAsync(
			"/Claims",
			new NewClaimDTO
			{
				CoverId = cover.Id,
				Name = "",
				Type = ClaimType.Collision,
				Created = coverEnd.AddDays(1), // after end
				DamageCost = 1,
			},
			cancellationToken: TestContext.Current.CancellationToken
		);

		Assert.False(response.IsSuccessStatusCode);
	}

	[Fact]
	public async Task RefusesZeroCostClaims()
	{
		using var testAppContext = new TestAppContext();

		var coverStart = DateTime.UtcNow;
		var coverEnd = DateTime.UtcNow.AddDays(1);
		var cover = new Cover(
			new NewCoverDTO
			{
				EndDate = coverEnd,
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
				Created = coverStart.AddMicroseconds(1),
				DamageCost = 0,
			},
			cancellationToken: TestContext.Current.CancellationToken
		);

		Assert.False(response.IsSuccessStatusCode);
	}

	[Fact]
	public async Task RefusesNegativeCostClaims()
	{
		using var testAppContext = new TestAppContext();

		var coverStart = DateTime.UtcNow;
		var coverEnd = DateTime.UtcNow.AddDays(1);
		var cover = new Cover(
			new NewCoverDTO
			{
				EndDate = coverEnd,
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
				Created = coverStart.AddMicroseconds(1),
				DamageCost = -1,
			},
			cancellationToken: TestContext.Current.CancellationToken
		);

		Assert.False(response.IsSuccessStatusCode);
	}

	[Fact]
	public async Task RefusesTooExpensiveClaims()
	{
		using var testAppContext = new TestAppContext();

		var coverStart = DateTime.UtcNow;
		var coverEnd = DateTime.UtcNow.AddDays(1);
		var cover = new Cover(
			new NewCoverDTO
			{
				EndDate = coverEnd,
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
				Created = coverStart.AddMicroseconds(1),
				DamageCost = (decimal)Claim.MaxCost + 1,
			},
			cancellationToken: TestContext.Current.CancellationToken
		);

		Assert.False(response.IsSuccessStatusCode);
	}
}
