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

public class QwenClient : ILlmClient
{
    private readonly HttpClient _http;
    private readonly ILlmResponseParser _parser;
    private readonly QwenOptions _options;
    private readonly ILogger _logger = Log.ForContext<QwenClient>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public LlmProvider Provider => LlmProvider.Qwen;

    public QwenClient(HttpClient http, ILlmResponseParser parser, IOptions<AnalyticsLlmOptions> options)
    {
        _http = http;
        _parser = parser;
        _options = options.Value.Qwen;

        _http.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
        _http.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        _http.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
        _http.DefaultRequestHeaders.Add("Origin", "https://chat.qwen.ai");
        _http.DefaultRequestHeaders.Add("Referer", "https://chat.qwen.ai/");
        _http.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
    }

    public async Task<ActionResult<LlmInsightResponse>> GenerateInsightsAsync(
        AnalyticsSnapshot snapshot, CancellationToken ct)
    {
        var snapshotJson = JsonSerializer.Serialize(snapshot, JsonOptions);
        var userContent = $"Данные:\n{snapshotJson}\n\nВерни JSON по схеме:\n{InsightSystemPrompt.ResponseSchema}";

        var requestBody = new
        {
            model = _options.Model,
            messages = new[]
            {
                new { role = "system", content = InsightSystemPrompt.Russian },
                new { role = "user", content = userContent }
            },
            temperature = _options.Temperature,
            max_tokens = _options.MaxTokens,
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _http.DefaultRequestHeaders.Remove("Authorization");
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.AccessToken}");

        _logger.Debug("Qwen request: {Chars} chars", json.Length);

        try
        {
            using var response = await _http.PostAsync("chat/completions", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.Warning("Qwen HTTP {Status}: {Body}", response.StatusCode, body);
                return ActionResult<LlmInsightResponse>.Failure($"analytics.llm.http_error:{(int)response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            var text = result
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return _parser.Parse(text);
        }
        catch (TaskCanceledException)
        {
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.timeout");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Qwen unexpected error");
            return ActionResult<LlmInsightResponse>.Failure("analytics.llm.unexpected_error");
        }
    }
}
