using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Shared.Llm;

public interface IGenericLlmClient
{
    LlmProvider Provider { get; }
    Task<string> CompleteAsync(string systemPrompt, string userMessage, CancellationToken ct);
}
