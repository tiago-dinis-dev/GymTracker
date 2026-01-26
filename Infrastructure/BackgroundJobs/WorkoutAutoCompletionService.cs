using Application.Workouts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.BackgroundJobs;

public class WorkoutAutoCompletionService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<AutoCompleteWorkoutHandler>();
                await handler.HandleAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
