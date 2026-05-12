using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace Api.IntegrationTests;

public class GymTrackerFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private readonly string _connectionString;

    public GymTrackerFactory()
    {
        try
        {
            _postgres.StartAsync().GetAwaiter().GetResult();
            _connectionString = _postgres.GetConnectionString();

            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Container started but GetConnectionString() returned empty.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to start PostgreSQL test container. Is Docker / Rancher Desktop running?", ex);
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
            // Both contexts share the same database — table names don't conflict.
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "integration-test-secret-key-must-be-at-least-32-chars",
                ["Jwt:Issuer"] = "GymTracker",
                ["Jwt:Audience"] = "GymTracker",
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["ConnectionStrings:DefaultConnectionAI"] = _connectionString,
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove background services — not relevant for integration tests
            var hostedServices = services.Where(d => d.ServiceType == typeof(IHostedService)).ToList();
            foreach (var svc in hostedServices) services.Remove(svc);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _postgres.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
