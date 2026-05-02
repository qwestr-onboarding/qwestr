using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.AI.Models;
using Qwestr.Modules.Analytics.Application.AI.Parsing;
using Qwestr.Modules.Analytics.Application.AI.Prompts;
using Qwestr.Modules.Analytics.Infrastructure.Configuration;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Serilog;

namespace Qwestr.Modules.Analytics.Infrastructure.AI;

public class YandexGptClient : ILlmClient
{
    private readonly HttpClient _http;
    private readonly ILlmResponseParser _parser;
    private readonly YandexGptOptions _options;
    private readonly ILogger _logger = Log.ForContext<YandexGptClient>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public LlmProvider Provider => LlmProvider.YandexGpt;

    public YandexGptClient(HttpClient http, ILlmResponseParser parser, IOptions<AnalyticsLlmOptions> options)
    {
        _http = http;
        _parser = parser;
        _options = options.Value.YandexGpt;
        _http.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<ActionResult<LlmInsightResponse>> GenerateInsightsAsync(
        AnalyticsSnapshot snapshot, CancellationToken ct)
    {
        var snapshotJson = JsonSerializer.Serialize(snapshot, JsonOptions);
        var userContent = $"Данные:\n{snapshotJson}\n\nВерни JSON по схеме:\n{InsightSystemPrompt.ResponseSchema}";

        var requestBody = new
        {
            modelUri = _options.ModelUri,
            completionOptions = new
            {
                stream = false,
                temperature = _options.Temperature,
                maxTokens = _options.MaxTokens.ToString()
            },
            messages = new[]
            {
                new { role = "system", text = InsightSystemPrompt.Russian },
                new { role = "user", text = userContent }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _http.DefaultRequestHeaders.Remove("Authorization");
        _http.DefaultRequestHeaders.Add("Authorization", $"Api-Key {_options.ApiKey}");

        _logger.Debug("YandexGPT request: {Chars} chars", json.Length);

        try
        {
            using var response = await _http.PostAsync(_options.Endpoint, content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.Warning("YandexGPT HTTP {Status}: {Body}", response.StatusCode, body);
                return ActionResult<LlmInsightResponse>.Failure($"analytics.llm.http_error:{(int)response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            var text = result
                .GetProperty("result")
                .GetProperty("alternatives")[0]
                .GetProperty("message")
                .GetProperty("text")
                .GetString() ?? string.Empty;

            return _parser.Parse(text);
        }
        catch (TaskCanceledException)
        {
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.timeout");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "YandexGPT unexpected error");
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.unexpected_error");
        }
    }
}
