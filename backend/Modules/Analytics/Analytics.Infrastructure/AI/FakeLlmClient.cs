using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.AI.Models;
using Qwestr.Modules.Analytics.Application.AI.Parsing;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Infrastructure.AI;

public class FakeLlmClient : ILlmClient
{
    private readonly ILlmResponseParser _parser;

    public LlmProvider Provider => LlmProvider.YandexGpt;

    public FakeLlmClient(ILlmResponseParser parser)
    {
        _parser = parser;
    }

    public Task<ActionResult<LlmInsightResponse>> GenerateInsightsAsync(
        AnalyticsSnapshot snapshot, CancellationToken ct)
    {
        const string fakeJson = """
        {
          "insights": [
            {
              "title": "Тестовый инсайт",
              "description": "Это тестовый инсайт от FakeLlmClient для локальной разработки.",
              "severity": "Info",
              "metricReference": "overall"
            }
          ],
          "recommendations": [
            {
              "title": "Тестовая рекомендация",
              "description": "Это тестовая рекомендация от FakeLlmClient для локальной разработки.",
              "category": "Process",
              "priority": 5
            }
          ]
        }
        """;

        var result = _parser.Parse(fakeJson);
        return Task.FromResult(result);
    }
}
