using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Application.AI.Abstractions;

public interface ILlmClientFactory
{
    ILlmClient Create(LlmProvider provider);
    IReadOnlyList<LlmProvider> GetAvailableProviders();
}
