using Qwestr.Modules.Onboarding.Application.AI.Models;

namespace Qwestr.Modules.Onboarding.Application.AI.Abstractions;

public interface IOnboardingScenarioGenerator
{
    Task<GeneratedScenarioDraft> GenerateAsync(ScenarioGenerationRequest request, CancellationToken ct);
}
