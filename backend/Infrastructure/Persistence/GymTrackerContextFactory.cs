using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class GymTrackerDbContextFactory : IDesignTimeDbContextFactory<GymTrackerDbContext>
{
    public GymTrackerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GymTrackerDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=gymtracker;Username=postgres;Password=postgres");

        return new GymTrackerDbContext(optionsBuilder.Options);
    }
}
