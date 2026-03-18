using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Onboarding.Api.Endpoints;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Application.Behaviors;
using Qwestr.Modules.Onboarding.Application.Commands.CreateScenario;
using Qwestr.Modules.Onboarding.Infrastructure;
using Qwestr.Modules.Onboarding.Infrastructure.Persistence.Repositories;

namespace Qwestr.Modules.Onboarding.Api;

public static class OnboardingStartup
{
    public static IServiceCollection AddOnboardingModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OnboardingDbContext>(o =>
            o.UseNpgsql(
                config.GetConnectionString("Onboarding"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "onboarding"))
             .UseSnakeCaseNamingConvention());

        services.AddScoped<IScenarioRepository, ScenarioRepository>();
        services.AddScoped<IScenarioAssignmentRepository, ScenarioAssignmentRepository>();

        services.AddAuthorization(o =>
            o.AddPolicy(OnboardingEndpoints.HrPolicy, p => p.RequireRole("Manager", "Admin", "Owner")));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateScenarioCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreateScenarioCommand).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IEndpointRouteBuilder MapOnboardingModule(this IEndpointRouteBuilder app)
    {
        app.MapOnboardingEndpoints();
        return app;
    }
}
