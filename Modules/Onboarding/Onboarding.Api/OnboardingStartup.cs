using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onboarding.Api.Endpoints;
using Onboarding.Infrastructure;

namespace Onboarding.Api;

public static class OnboardingStartup
{
    public static IServiceCollection AddOnboardingModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OnboardingDbContext>(o => o.UseNpgsql(config.GetConnectionString("Onboarding")));
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