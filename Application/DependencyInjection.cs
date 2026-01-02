using Application.Workouts;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AddExerciseToWorkoutHandler>();
        services.AddScoped<CreateWorkoutHandler>();
        services.AddScoped<CompleteWorkoutHandler>();
        services.AddScoped<GetWorkoutHistoryHandler>();
        services.AddScoped<GetWorkoutByIdHandler>();

        return services;
    }
}
