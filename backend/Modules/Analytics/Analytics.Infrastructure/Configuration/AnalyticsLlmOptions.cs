using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Infrastructure.Configuration;

public class AnalyticsLlmOptions
{
    public const string SectionName = "Analytics:Llm";

    public LlmProvider DefaultProvider { get; set; } = LlmProvider.YandexGpt;
    public bool UseFake { get; set; } = false;
    public YandexGptOptions YandexGpt { get; set; } = new();
    public GeminiOptions Gemini { get; set; } = new();
}

public class YandexGptOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string FolderId { get; set; } = string.Empty;
    public string ModelUri { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0.2;
    public int MaxTokens { get; set; } = 2000;
}

public class GeminiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
    public string Endpoint { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0.2;
}

public class AnalyticsBackgroundJobsOptions
{
    public const string SectionName = "Analytics:BackgroundJobs";

    public bool DailySnapshotEnabled { get; set; } = true;
    public string DailySnapshotTimeUtc { get; set; } = "02:00";
    public bool DailyInsightWarmupEnabled { get; set; } = true;
    public string DailyInsightWarmupTimeUtc { get; set; } = "06:00";
}
