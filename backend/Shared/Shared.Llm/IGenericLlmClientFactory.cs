using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Shared.Llm;

public interface IGenericLlmClientFactory
{
    IGenericLlmClient Create(LlmProvider provider);
    IReadOnlyList<LlmProvider> GetAvailableProviders();
}
