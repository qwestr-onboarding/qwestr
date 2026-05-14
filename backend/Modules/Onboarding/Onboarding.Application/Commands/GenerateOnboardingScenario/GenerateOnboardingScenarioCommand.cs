using MediatR;
using Qwestr.Modules.Onboarding.Application.AI.Models;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Onboarding.Application.Commands.GenerateOnboardingScenario;

public record GenerateOnboardingScenarioCommand(
    Guid TenantId,
    Guid UserId,
    string Query,
    List<AttachedDocument>? Documents,
    int StageCount,
    int StepsPerStage,
    string TextStyle,
    string? AdditionalInfo,
    string? TargetAudience,
    bool IncludeFinalTest,
    int? TotalPoints,
    LlmProvider? Provider) : IRequest<ActionResult<GenerateOnboardingScenarioResult>>;

public record GenerateOnboardingScenarioResult(Guid ScenarioId, string AiComment);
