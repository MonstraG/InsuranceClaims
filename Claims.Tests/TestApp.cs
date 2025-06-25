using System.Text.Json.Serialization;
using Claims;
using Claims.Auditing;
using Claims.Claims;
using Claims.Claims.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder
	.Services.AddControllers()
	.AddJsonOptions(x =>
	{
		x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});

builder.Services.AddDbContextFactory<AuditContext>(options =>
	options.UseInMemoryDatabase(Guid.NewGuid().ToString())
);

builder.Services.AddDbContext<ClaimsContext>(options =>
	options.UseInMemoryDatabase(Guid.NewGuid().ToString())
);

// this is needed for this TestApp to pick up real controllers
var startupAssembly = typeof(Claim).Assembly;
builder.Services.AddControllers().AddApplicationPart(startupAssembly).AddControllersAsServices();

builder.Services.AddMyServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class TestApp;
