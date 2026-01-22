namespace Application.AI.Abstractions;

public interface IAiObservationStore<in T>
{
    Task AddAsync(T observation, CancellationToken cancellationToken);
}
