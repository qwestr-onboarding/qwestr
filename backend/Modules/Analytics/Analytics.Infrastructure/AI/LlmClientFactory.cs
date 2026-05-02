using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Infrastructure.Configuration;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Microsoft.Extensions.Options;

namespace Qwestr.Modules.Analytics.Infrastructure.AI;

public class LlmClientFactory : ILlmClientFactory
{
    private readonly IServiceProvider _sp;
    private readonly AnalyticsLlmOptions _options;

    public LlmClientFactory(IServiceProvider sp, IOptions<AnalyticsLlmOptions> options)
    {
        _sp = sp;
        _options = options.Value;
    }

    public ILlmClient Create(LlmProvider provider)
    {
        if (_options.UseFake)
        {
            return (ILlmClient)_sp.GetService(typeof(FakeLlmClient))!;
        }

        return provider switch
        {
            LlmProvider.YandexGpt => (ILlmClient)_sp.GetService(typeof(YandexGptClient))!,
            LlmProvider.Gemini => (ILlmClient)_sp.GetService(typeof(GeminiClient))!,
            LlmProvider.Qwen => (ILlmClient)_sp.GetService(typeof(QwenClient))!,
            _ => throw new InvalidOperationException($"Unknown LLM provider: {provider}")
        };
    }

    public IReadOnlyList<LlmProvider> GetAvailableProviders()
    {
        return Enum.GetValues<LlmProvider>().ToList();
    }
}
