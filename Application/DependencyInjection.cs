using Application.Common;
using Application.Common.Interfaces;
using Application.Users;
using Application.Workouts;
using Domain.Common.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateUserHandler>();
        services.AddScoped<GetUserByEmailHandler>();
        services.AddScoped<UpdateUserHandler>();

        services.AddScoped<AddExerciseToWorkoutHandler>();
        services.AddScoped<AutoCompleteWorkoutHandler>();
        services.AddScoped<CreateWorkoutHandler>();
        services.AddScoped<CompleteWorkoutHandler>();
        services.AddScoped<UpdateExerciseSetInWorkoutHandler>();
        services.AddScoped<GetWorkoutHistoryHandler>();
        services.AddScoped<GetWorkoutByIdHandler>();
        services.AddScoped<IDomainEventHandler<WorkoutCompleted>, WorkoutCompletedCacheHandler>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
