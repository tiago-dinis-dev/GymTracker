using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AI.Persistence;

public class AIObservationDbContext(DbContextOptions<AIObservationDbContext> options) : DbContext(options)
{
}
