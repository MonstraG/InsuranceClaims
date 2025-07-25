using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Claims.Auditing;
using Claims.Claims;
using Claims.Claims.Models;
using Claims.Claims.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Serilog;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace Claims;

public class Program
{
	public static async Task Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

		try
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddSerilog();

			// Start Testcontainers for SQL Server and MongoDB
			var sqlContainer = (
				RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
					? new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
					: new MsSqlBuilder()
			).Build();

			var mongoContainer = new MongoDbBuilder().WithImage("mongo:latest").Build();

			var sqlContainerStartTask = sqlContainer.StartAsync();
			var mongoContainerStartTask = mongoContainer.StartAsync();

			await Task.WhenAll(sqlContainerStartTask, mongoContainerStartTask);

			// Add services to the container.
			builder
				.Services.AddControllers()
				.AddJsonOptions(x =>
				{
					x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				});

			builder.Services.AddDbContextFactory<AuditContext>(options =>
				options.UseSqlServer(sqlContainer.GetConnectionString())
			);

			builder.Services.AddDbContext<ClaimsContext>(options =>
			{
				var client = new MongoClient(mongoContainer.GetConnectionString());
				var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]); // Use a default/test database name
				options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
			});

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddMyServices();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseSerilogRequestLogging();

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			using (var scope = app.Services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
				await context.Database.MigrateAsync();
			}

			await app.RunAsync();
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Application terminated unexpectedly");
		}
		finally
		{
			await Log.CloseAndFlushAsync();
		}
	}
}
