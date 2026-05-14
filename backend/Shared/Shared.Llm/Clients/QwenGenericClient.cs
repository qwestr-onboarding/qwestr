using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Qwestr.Shared.Llm.Options;
using Serilog;

namespace Qwestr.Shared.Llm.Clients;

public class QwenGenericClient : IGenericLlmClient
{
    private readonly HttpClient _http;
    private readonly QwenOptions _options;
    private readonly ILogger _logger = Log.ForContext<QwenGenericClient>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public LlmProvider Provider => LlmProvider.Qwen;

    public QwenGenericClient(HttpClient http, IOptions<LlmSharedOptions> options)
    {
        _http = http;
        _options = options.Value.Qwen;

        _http.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
        _http.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> CompleteAsync(string systemPrompt, string userMessage, CancellationToken ct)
    {
        var requestBody = new
        {
            model = _options.Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            temperature = _options.Temperature,
            max_tokens = _options.MaxTokens,
            extra_body= new
            {
                enable_thinking = false
            }
        };

        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.Debug("Qwen request: {Chars} chars", json.Length);

        // Use a dedicated timeout so a short-lived ASP.NET/MediatR CancellationToken
        // does not abort the request before HttpClient.Timeout fires.
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        using var response = await _http.PostAsync("chat/completions", content, linkedCts.Token);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(linkedCts.Token);
            _logger.Warning("Qwen HTTP {Status}: {Body}", response.StatusCode, body);
            throw new HttpRequestException($"Qwen HTTP {(int)response.StatusCode}: {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: linkedCts.Token);
        return result
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;
    }
}
