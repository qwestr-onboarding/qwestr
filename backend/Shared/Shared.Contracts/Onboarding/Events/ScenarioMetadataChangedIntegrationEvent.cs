using MediatR;

namespace Qwestr.Shared.Contracts.Onboarding.Events;

public record ScenarioMetadataChangedIntegrationEvent(
    Guid TenantId,
    Guid ScenarioId,
    string ScenarioName,
    IReadOnlyList<ScenarioStageMetadataDto> Stages
) : INotification;

public record ScenarioStageMetadataDto(
    Guid StageId,
    string StageName,
    int Order,
    IReadOnlyList<ScenarioStepMetadataDto> Steps);

public record ScenarioStepMetadataDto(
    Guid StepId,
    string StepName,
    int Order);