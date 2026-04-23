using Application.AI.Abstractions;
using Common.AI.Observations;

namespace Infrastructure.AI.Persistence;

public class AIObservationStore(AIObservationDbContext dbContext) : IAIObservationStore
{
    private readonly AIObservationDbContext _dbContext = dbContext;
    public async Task AddAsync(WorkoutCompletedObservation observation, CancellationToken cancellationToken)
    {
        await _dbContext.WorkoutCompletedObservations.AddAsync(observation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAsync(ExerciseAddedObservation observation, CancellationToken cancellationToken)
    {
        await _dbContext.ExerciseAddedObservations.AddAsync(observation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
