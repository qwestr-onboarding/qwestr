namespace Qwestr.Modules.Analytics.Infrastructure.Configuration;

public class AnalyticsLlmOptions
{
    public const string SectionName = "Analytics:Llm";

    public int InsightCooldownHours { get; set; } = 0;
}

public class AnalyticsBackgroundJobsOptions
{
    public const string SectionName = "Analytics:BackgroundJobs";

    public bool DailySnapshotEnabled { get; set; } = true;
    public string DailySnapshotTimeUtc { get; set; } = "02:00";
    public bool DailyInsightWarmupEnabled { get; set; } = true;
    public string DailyInsightWarmupTimeUtc { get; set; } = "06:00";
}
