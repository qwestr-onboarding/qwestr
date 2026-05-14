using System.Text.Encodings.Web;
using System.Text.Json;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.AI.Models;
using Qwestr.Modules.Analytics.Application.AI.Parsing;
using Qwestr.Modules.Analytics.Application.AI.Prompts;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Qwestr.Shared.Llm;
using Serilog;

namespace Qwestr.Modules.Analytics.Infrastructure.AI;

public class AnalyticsLlmClientAdapter : ILlmClient
{
    private readonly IGenericLlmClient _inner;
    private readonly ILlmResponseParser _parser;
    private readonly ILogger _logger = Log.ForContext<AnalyticsLlmClientAdapter>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public LlmProvider Provider => _inner.Provider;

    public AnalyticsLlmClientAdapter(IGenericLlmClient inner, ILlmResponseParser parser)
    {
        _inner = inner;
        _parser = parser;
    }

    public async Task<ActionResult<LlmInsightResponse>> GenerateInsightsAsync(
        AnalyticsSnapshot snapshot, CancellationToken ct)
    {
        var snapshotJson = JsonSerializer.Serialize(snapshot, JsonOptions);
        var userContent = $"Данные:\n{snapshotJson}\n\nВерни JSON по схеме:\n{InsightSystemPrompt.ResponseSchema}";

        try
        {
            var raw = await _inner.CompleteAsync(InsightSystemPrompt.Russian, userContent, ct);
            return _parser.Parse(raw);
        }
        catch (TaskCanceledException)
        {
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.timeout");
        }
        catch (HttpRequestException ex)
        {
            _logger.Warning(ex, "LLM HTTP error via {Provider}", _inner.Provider);
            return ActionResult<LlmInsightResponse>.Failure($"analytics.llm.http_error");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "LLM unexpected error via {Provider}", _inner.Provider);
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.unexpected_error");
        }
    }
}
