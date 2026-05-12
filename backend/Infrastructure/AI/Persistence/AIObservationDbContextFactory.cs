using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.AI.Persistence;

public class AIObservationDbContextFactory : IDesignTimeDbContextFactory<AIObservationDbContext>
{
    public AIObservationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AIObservationDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=gymtracker_ai;Username=postgres;Password=postgres");

        return new AIObservationDbContext(optionsBuilder.Options);
    }
}
