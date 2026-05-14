using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Qwestr.Shared.Llm.Options;
using Serilog;

namespace Qwestr.Shared.Llm.Clients;

public class YandexGptGenericClient : IGenericLlmClient
{
    private readonly HttpClient _http;
    private readonly YandexGptOptions _options;
    private readonly ILogger _logger = Log.ForContext<YandexGptGenericClient>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public LlmProvider Provider => LlmProvider.YandexGpt;

    public YandexGptGenericClient(HttpClient http, IOptions<LlmSharedOptions> options)
    {
        _http = http;
        _options = options.Value.YandexGpt;
        _http.Timeout = TimeSpan.FromSeconds(60);
    }

    public async Task<string> CompleteAsync(string systemPrompt, string userMessage, CancellationToken ct)
    {
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
                new { role = "system", text = systemPrompt },
                new { role = "user", text = userMessage }
            }
        };

        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _http.DefaultRequestHeaders.Remove("Authorization");
        _http.DefaultRequestHeaders.Add("Authorization", $"Api-Key {_options.ApiKey}");

        _logger.Debug("YandexGPT request: {Chars} chars", json.Length);

        using var response = await _http.PostAsync(_options.Endpoint, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.Warning("YandexGPT HTTP {Status}: {Body}", response.StatusCode, body);
            throw new HttpRequestException($"YandexGPT HTTP {(int)response.StatusCode}: {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        return result
            .GetProperty("result")
            .GetProperty("alternatives")[0]
            .GetProperty("message")
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}
