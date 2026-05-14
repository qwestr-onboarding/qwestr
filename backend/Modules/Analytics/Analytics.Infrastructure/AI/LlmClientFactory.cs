using Microsoft.Extensions.Options;
using Qwestr.Modules.Analytics.Application.AI.Abstractions;
using Qwestr.Modules.Analytics.Application.AI.Parsing;
using Qwestr.Modules.Analytics.Infrastructure.Configuration;
using Qwestr.Shared.Kernel.Enums.Analytics;
using Qwestr.Shared.Llm;
using Qwestr.Shared.Llm.Options;

namespace Qwestr.Modules.Analytics.Infrastructure.AI;

public class LlmClientFactory : ILlmClientFactory
{
    private readonly IGenericLlmClientFactory _genericFactory;
    private readonly ILlmResponseParser _parser;
    private readonly LlmSharedOptions _sharedOptions;

    public LlmClientFactory(
        IGenericLlmClientFactory genericFactory,
        ILlmResponseParser parser,
        IOptions<LlmSharedOptions> sharedOptions)
    {
        _genericFactory = genericFactory;
        _parser = parser;
        _sharedOptions = sharedOptions.Value;
    }

    public ILlmClient Create(LlmProvider provider)
    {
        var generic = _genericFactory.Create(provider);
        return new AnalyticsLlmClientAdapter(generic, _parser);
    }

    public IReadOnlyList<LlmProvider> GetAvailableProviders()
        => _genericFactory.GetAvailableProviders();
}
