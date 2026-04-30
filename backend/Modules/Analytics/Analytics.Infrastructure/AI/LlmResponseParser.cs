using System.Text.Json;
using System.Text.Json.Serialization;
using Qwestr.Modules.Analytics.Application.AI.Models;
using Qwestr.Modules.Analytics.Application.AI.Parsing;
using Qwestr.Modules.Analytics.Domain.Entities.Insights;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Serilog;

namespace Qwestr.Modules.Analytics.Infrastructure.AI;

public class LlmResponseParser : ILlmResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private readonly ILogger _logger = Log.ForContext<LlmResponseParser>();

    public ActionResult<LlmInsightResponse> Parse(string rawJson)
    {
        var json = ExtractJson(rawJson);
        if (json is null)
        {
            _logger.Error("LlmResponseParser: could not locate JSON in response. Raw: {Raw}", rawJson);
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.parse_failed");
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<LlmResponseDto>(json, JsonOptions);
            if (parsed is null)
            {
                return ActionResult<LlmInsightResponse>.Failure("analytics.llm.parse_failed");
            }

            var insights = parsed.Insights?.Select(i =>
            {
                if (!Enum.TryParse<InsightSeverity>(i.Severity, ignoreCase: true, out var severity))
                {
                    severity = InsightSeverity.Info;
                }
                return new InsightDraft(i.Title ?? "", i.Description ?? "", severity, i.MetricReference ?? "");
            }).ToList() ?? new List<InsightDraft>();

            var recommendations = parsed.Recommendations?.Select(r =>
            {
                if (!Enum.TryParse<RecommendationCategory>(r.Category, ignoreCase: true, out var category))
                {
                    category = RecommendationCategory.Process;
                }
                return new RecommendationDraft(r.Title ?? "", r.Description ?? "", category, r.Priority);
            }).ToList() ?? new List<RecommendationDraft>();

            return ActionResult<LlmInsightResponse>.Success(new LlmInsightResponse(insights, recommendations));
        }
        catch (JsonException ex)
        {
            _logger.Error(ex, "LlmResponseParser: JSON deserialization failed. Raw: {Raw}", rawJson);
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.parse_failed");
        }
    }

    private static string? ExtractJson(string raw)
    {
        var start = raw.IndexOf('{');
        var end = raw.LastIndexOf('}');
        if (start < 0 || end < 0 || end <= start)
        {
            return null;
        }
        return raw[start..(end + 1)];
    }

    private sealed class LlmResponseDto
    {
        public List<InsightItemDto>? Insights { get; set; }
        public List<RecommendationItemDto>? Recommendations { get; set; }
    }

    private sealed class InsightItemDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Severity { get; set; }
        public string? MetricReference { get; set; }
    }

    private sealed class RecommendationItemDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int Priority { get; set; }
    }
}
