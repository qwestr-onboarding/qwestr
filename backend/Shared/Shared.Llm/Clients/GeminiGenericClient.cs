using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Qwestr.Shared.Llm.Options;
using Serilog;

namespace Qwestr.Shared.Llm.Clients;

public class GeminiGenericClient : IGenericLlmClient
{
    private readonly GeminiOptions _options;
    private readonly ILogger _logger = Log.ForContext<GeminiGenericClient>();

    public LlmProvider Provider => LlmProvider.Gemini;

    public GeminiGenericClient(IOptions<LlmSharedOptions> options)
    {
        _options = options.Value.Gemini;
    }

    public async Task<string> CompleteAsync(string systemPrompt, string userMessage, CancellationToken ct)
    {
        _logger.Debug("Gemini request: {Chars} chars", userMessage.Length);

        var gemini = new Client(apiKey: _options.ApiKey);

        var config = new GenerateContentConfig
        {
            SystemInstruction = new Content
            {
                Parts = [new Part { Text = systemPrompt }]
            },
            ResponseMimeType = "application/json",
            Temperature = (float)_options.Temperature
        };

        var response = await gemini.Models.GenerateContentAsync(
            model: _options.Model,
            contents: userMessage,
            config: config,
            cancellationToken: ct);

        return response.Candidates![0].Content!.Parts![0].Text ?? string.Empty;
    }
}
