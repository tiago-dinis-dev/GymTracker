using Infrastructure.AI.Persistence;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Api.IntegrationTests;

public class GymTrackerFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _gymTrackerConnection = new("DataSource=:memory:");
    private readonly SqliteConnection _aiObservationConnection = new("DataSource=:memory:");

    public GymTrackerFactory()
    {
        _gymTrackerConnection.Open();
        _aiObservationConnection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "integration-test-secret-key-must-be-at-least-32-chars",
                ["Jwt:Issuer"] = "GymTracker",
                ["Jwt:Audience"] = "GymTracker",
                ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:",
                ["ConnectionStrings:DefaultConnectionAI"] = "DataSource=:memory:",
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // EF Core 9+ registers options actions as IDbContextOptionsConfiguration<T> via AddSingleton
            // (not TryAdd). A second AddDbContext call would accumulate both Npgsql + SQLite actions,
            // causing "two database providers" error. Solution: remove all option-related descriptors
            // for each context and inject pre-built SQLite options directly.
            ReplaceDbContext<GymTrackerDbContext>(services, _gymTrackerConnection);
            ReplaceDbContext<AIObservationDbContext>(services, _aiObservationConnection);

            // Remove background services — irrelevant for integration tests
            var hostedServices = services.Where(d => d.ServiceType == typeof(IHostedService)).ToList();
            foreach (var svc in hostedServices)
                services.Remove(svc);
        });
    }

    private static void ReplaceDbContext<TContext>(IServiceCollection services, SqliteConnection connection)
        where TContext : DbContext
    {
        // Remove factory-based DbContextOptions<T> registration
        services.RemoveAll(typeof(DbContextOptions<TContext>));

        // Remove the DbContext itself so AddDbContext can re-register it properly
        // (AddDbContext uses TryAdd, which skips if already registered)
        services.RemoveAll(typeof(TContext));

        // Remove IDbContextOptionsConfiguration<T> registrations added by AddDbContext (EF Core 9+).
        // These hold the provider-specific options actions (e.g. UseNpgsql) via AddSingleton (not TryAdd).
        // If not removed, both Npgsql + SQLite actions accumulate → "two database providers" error.
        var configDescriptors = services
            .Where(d =>
                d.ServiceType.IsGenericType &&
                d.ServiceType.GenericTypeArguments.Length == 1 &&
                d.ServiceType.GenericTypeArguments[0] == typeof(TContext) &&
                d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration"))
            .ToList();
        foreach (var d in configDescriptors) services.Remove(d);

        // Re-register via AddDbContext so EF Core sets up ApplicationServiceProvider correctly.
        // This is required for DbContext.GetService<T>() to fall back to the app DI container
        // (e.g. IDomainEventDispatcher used in GymTrackerDbContext.SaveChangesAsync).
        services.AddDbContext<TContext>(options => options.UseSqlite(connection));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _gymTrackerConnection.Dispose();
            _aiObservationConnection.Dispose();
        }
    }
}
