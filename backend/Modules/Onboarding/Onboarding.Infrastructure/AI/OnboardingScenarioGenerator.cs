using Microsoft.Extensions.Options;
using Qwestr.Modules.Onboarding.Application.AI.Abstractions;
using Qwestr.Modules.Onboarding.Application.AI.Models;
using Qwestr.Shared.Llm;
using Qwestr.Shared.Llm.Options;
using Serilog;

namespace Qwestr.Modules.Onboarding.Infrastructure.AI;

public class OnboardingScenarioGenerator : IOnboardingScenarioGenerator
{
    private readonly IGenericLlmClientFactory _factory;
    private readonly LlmSharedOptions _llmOptions;
    private readonly OnboardingScenarioPromptBuilder _promptBuilder;
    private readonly OnboardingScenarioResponseParser _parser;
    private readonly ILogger _logger = Log.ForContext<OnboardingScenarioGenerator>();

    public OnboardingScenarioGenerator(
        IGenericLlmClientFactory factory,
        IOptions<LlmSharedOptions> llmOptions,
        OnboardingScenarioPromptBuilder promptBuilder,
        OnboardingScenarioResponseParser parser)
    {
        _factory = factory;
        _llmOptions = llmOptions.Value;
        _promptBuilder = promptBuilder;
        _parser = parser;
    }

    public async Task<GeneratedScenarioDraft> GenerateAsync(ScenarioGenerationRequest request, CancellationToken ct)
    {
        var provider = request.Provider ?? _llmOptions.DefaultProvider;
        var client = _factory.Create(provider);

        var (systemPrompt, userMessage) = _promptBuilder.Build(request);

        _logger.Information("Generating onboarding scenario via {Provider}, stages={Stages}, steps={Steps}",
            provider, request.StageCount, request.StepsPerStage);

        var raw = await client.CompleteAsync(systemPrompt, userMessage, ct);
        return _parser.Parse(raw);
    }
}
