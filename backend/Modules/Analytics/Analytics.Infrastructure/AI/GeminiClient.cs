using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.AI.Models;
using Qwestr.Modules.Analytics.Application.AI.Parsing;
using Qwestr.Modules.Analytics.Application.AI.Prompts;
using Qwestr.Modules.Analytics.Infrastructure.Configuration;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Serilog;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Qwestr.Modules.Analytics.Infrastructure.AI;

public class GeminiClient : ILlmClient
{
    private readonly ILlmResponseParser _parser;
    private readonly GeminiOptions _options;
    private readonly ILogger _logger = Log.ForContext<GeminiClient>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public LlmProvider Provider => LlmProvider.Gemini;

    public GeminiClient(ILlmResponseParser parser, IOptions<AnalyticsLlmOptions> options)
    {
        _parser = parser;
        _options = options.Value.Gemini;
    }

    public async Task<ActionResult<LlmInsightResponse>> GenerateInsightsAsync(
        AnalyticsSnapshot snapshot, CancellationToken ct)
    {
        var snapshotJson = JsonSerializer.Serialize(snapshot, JsonOptions);
        var userContent = $"Данные:\n{snapshotJson}\n\nВерни JSON по схеме:\n{InsightSystemPrompt.ResponseSchema}";

        _logger.Debug("Gemini request: {Chars} chars", userContent.Length);

        try
        {
            var gemini = new Client(apiKey: _options.ApiKey);

            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts = [new Part { Text = InsightSystemPrompt.Russian }]
                },
                ResponseMimeType = "application/json",
                Temperature = (float)_options.Temperature
            };

            var response = await gemini.Models.GenerateContentAsync(
                model: _options.Model,
                contents: userContent,
                config: config,
                cancellationToken: ct);

            var text = response.Candidates![0].Content!.Parts![0].Text ?? string.Empty;
            return _parser.Parse(text);
        }
        catch (TaskCanceledException)
        {
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.timeout");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Gemini unexpected error");
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.unexpected_error");
        }
    }
}
