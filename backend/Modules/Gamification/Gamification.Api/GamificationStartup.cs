using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Gamification.Api.Endpoints;
using Qwestr.Modules.Gamification.Application;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Application.Abstractions.Services;
using Qwestr.Modules.Gamification.Infrastructure;
using Qwestr.Modules.Gamification.Infrastructure.Persistence.Repositories;
using Qwestr.Modules.Gamification.Infrastructure.Services;

namespace Qwestr.Modules.Gamification.Api;

public static class GamificationStartup
{
    public static IServiceCollection AddGamificationModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<GamificationDbContext>(o =>
            o.UseNpgsql(
                config.GetConnectionString("Gamification"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "gamification"))
             .UseSnakeCaseNamingConvention());

        services.AddScoped<IPlayerProfileRepository, PlayerProfileRepository>();
        services.AddScoped<IAwardRepository, AwardRepository>();
        services.AddScoped<IShopItemRepository, ShopItemRepository>();
        services.AddScoped<ILevelTierRepository, LevelTierRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IPointsTransactionRepository, PointsTransactionRepository>();

        services.AddScoped<ILevelCalculator, LevelCalculator>();
        services.AddScoped<ITenantConfigurationProvider, TenantConfigurationProvider>();
        services.AddScoped<IFileStorageService, GamificationFileStorageService>();

        services.AddGamificationApplication();

        services.AddAuthorization(o =>
            o.AddPolicy(GamificationEndpoints.HrPolicy, p => p.RequireRole("Manager", "Admin", "Owner")));

        return services;
    }

    public static IEndpointRouteBuilder MapGamificationModule(this IEndpointRouteBuilder app)
    {
        app.MapGamificationEndpoints();
        return app;
    }
}
