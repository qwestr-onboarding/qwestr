using MediatR;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetAiGenerationStatus;

public record GetAiGenerationStatusQuery(Guid TenantId) : IRequest<AiGenerationStatusDto>;

public record AiGenerationStatusDto(
    bool CanGenerate,
    int UsedToday,
    int MaxPerDay,
    DateTime? NextAvailableAt);
