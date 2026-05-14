using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Qwestr.Shared.Llm.Clients;
using Qwestr.Shared.Llm.Options;

namespace Qwestr.Shared.Llm;

public class GenericLlmClientFactory : IGenericLlmClientFactory
{
    private readonly IServiceProvider _sp;
    private readonly LlmSharedOptions _options;

    public GenericLlmClientFactory(IServiceProvider sp, IOptions<LlmSharedOptions> options)
    {
        _sp = sp;
        _options = options.Value;
    }

    public IGenericLlmClient Create(LlmProvider provider)
    {
        if (_options.UseFake)
            return _sp.GetRequiredService<FakeGenericLlmClient>();

        return provider switch
        {
            LlmProvider.YandexGpt => _sp.GetRequiredService<YandexGptGenericClient>(),
            LlmProvider.Gemini => _sp.GetRequiredService<GeminiGenericClient>(),
            LlmProvider.Qwen => _sp.GetRequiredService<QwenGenericClient>(),
            _ => throw new InvalidOperationException($"Unknown LLM provider: {provider}")
        };
    }

    public IReadOnlyList<LlmProvider> GetAvailableProviders()
        => Enum.GetValues<LlmProvider>().ToList();
}
