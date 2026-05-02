using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qwestr.Modules.Analytics.Application.Abstractions.Persistence;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.AI.Parsing;
using Qwestr.Modules.Analytics.Application.Options;
using Qwestr.Modules.Analytics.Application.Services;
using Qwestr.Modules.Analytics.Infrastructure.AI;
using Qwestr.Modules.Analytics.Infrastructure.BackgroundJobs;
using Qwestr.Modules.Analytics.Infrastructure.Configuration;
using Qwestr.Modules.Analytics.Infrastructure.Persistence.Repositories;
using Qwestr.Modules.Analytics.Infrastructure.Services;

namespace Qwestr.Modules.Analytics.Infrastructure;

public static class AnalyticsInfrastructureExtensions
{
    public static IServiceCollection AddAnalyticsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AnalyticsDbContext>(o =>
            o.UseNpgsql(
                configuration.GetConnectionString("Analytics"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "analytics"))
             .UseSnakeCaseNamingConvention());

        services.AddScoped<IStepCompletionFactRepository, StepCompletionFactRepository>();
        services.AddScoped<IScenarioAssignmentFactRepository, ScenarioAssignmentFactRepository>();
        services.AddScoped<IDailyEngagementSnapshotRepository, DailyEngagementSnapshotRepository>();
        services.AddScoped<IScenarioMetadataCacheRepository, ScenarioMetadataCacheRepository>();
        services.AddScoped<IAnalyticsInsightSetRepository, AnalyticsInsightSetRepository>();

        services.Configure<AnalyticsLlmOptions>(
            configuration.GetSection(AnalyticsLlmOptions.SectionName));
        services.Configure<AnalyticsInsightOptions>(
            configuration.GetSection(AnalyticsInsightOptions.SectionName));
        services.Configure<AnalyticsBackgroundJobsOptions>(
            configuration.GetSection(AnalyticsBackgroundJobsOptions.SectionName));

        services.AddSingleton<ILlmResponseParser, LlmResponseParser>();
        services.AddScoped<FakeLlmClient>();
        services.AddHttpClient<YandexGptClient>();
        services.AddScoped<GeminiClient>();
        services.AddHttpClient<QwenClient>();
        services.AddScoped<ILlmClientFactory, LlmClientFactory>();

        services.AddScoped<IAnalyticsSnapshotBuilder, AnalyticsSnapshotBuilder>();
        services.AddSingleton<ISnapshotHasher, SnapshotHasher>();
        services.AddScoped<IDailyMetricsAccumulator, DailyMetricsAccumulator>();

        services.AddHostedService<DailySnapshotJob>();
        services.AddHostedService<DailyInsightWarmupJob>();

        return services;
    }
}
