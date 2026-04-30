using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Qwestr.Modules.Analytics.Application;

public static class AnalyticsApplicationExtensions
{
    public static IServiceCollection AddAnalyticsApplication(this IServiceCollection services)
    {
        var assembly = typeof(AnalyticsApplicationExtensions).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
