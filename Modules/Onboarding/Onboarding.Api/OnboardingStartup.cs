using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Onboarding.Api.Endpoints;
using Qwestr.Modules.Onboarding.Infrastructure;

namespace Qwestr.Modules.Onboarding.Api;

public static class OnboardingStartup
{
    public static IServiceCollection AddOnboardingModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OnboardingDbContext>(o => o.UseNpgsql(config.GetConnectionString("Onboarding")).UseSnakeCaseNamingConvention());
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OnboardingStartup).Assembly));
        return services;
    }
    
    public static IEndpointRouteBuilder MapOnboardingModule(
        this IEndpointRouteBuilder app)
    {
        app.MapOnboardingEndpoints();
        return app;
    }
}