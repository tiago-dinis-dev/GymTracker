using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class GymTrackerDbContextFactory : IDesignTimeDbContextFactory<GymTrackerDbContext>
{
    public GymTrackerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GymTrackerDbContext>();

        optionsBuilder.UseSqlite("Data Source=gymtracker.db");

        return new GymTrackerDbContext(optionsBuilder.Options);
    }
}
