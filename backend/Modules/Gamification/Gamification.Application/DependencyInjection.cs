using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Gamification.Application.Awarding;
using Qwestr.Modules.Gamification.Application.Awarding.Evaluators;
using Qwestr.Modules.Gamification.Application.Behaviors;
using Qwestr.Modules.Gamification.Application.Points;

namespace Qwestr.Modules.Gamification.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGamificationApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IPointsService, PointsService>();
        services.AddScoped<IAwardEvaluator, AwardEvaluator>();

        services.AddScoped<IRuleEvaluator, StepCompletedCountEvaluator>();
        services.AddScoped<IRuleEvaluator, ScenarioCompletedCountEvaluator>();
        services.AddScoped<IRuleEvaluator, TestPassedWithFilterEvaluator>();
        services.AddScoped<IRuleEvaluator, LevelThresholdEvaluator>();
        services.AddScoped<IRuleEvaluator, PointsAccumulatedEvaluator>();

        return services;
    }
}
