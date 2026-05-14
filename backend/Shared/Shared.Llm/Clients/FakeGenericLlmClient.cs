using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Shared.Llm.Clients;

public class FakeGenericLlmClient : IGenericLlmClient
{
    private readonly string _fakeResponse;

    public LlmProvider Provider => LlmProvider.YandexGpt;

    public FakeGenericLlmClient(string fakeResponse = "{}")
    {
        _fakeResponse = fakeResponse;
    }

    public Task<string> CompleteAsync(string systemPrompt, string userMessage, CancellationToken ct)
        => Task.FromResult(_fakeResponse);
}
