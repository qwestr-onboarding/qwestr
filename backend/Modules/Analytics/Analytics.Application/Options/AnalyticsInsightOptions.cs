namespace Qwestr.Modules.Analytics.Application.Options;

public class AnalyticsInsightOptions
{
    public const string SectionName = "Analytics:Llm";

    public int InsightCooldownHours { get; set; } = 5;
}
