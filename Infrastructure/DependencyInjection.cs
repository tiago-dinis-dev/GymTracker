using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<GymTrackerDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
