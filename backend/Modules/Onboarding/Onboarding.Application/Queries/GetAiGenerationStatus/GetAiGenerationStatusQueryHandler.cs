using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Application.Options;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetAiGenerationStatus;

public class GetAiGenerationStatusQueryHandler : IRequestHandler<GetAiGenerationStatusQuery, AiGenerationStatusDto>
{
    private readonly IAiGenerationQuotaRepository _quotaRepository;
    private readonly OnboardingAiOptions _options;

    public GetAiGenerationStatusQueryHandler(
        IAiGenerationQuotaRepository quotaRepository,
        IOptions<OnboardingAiOptions> options)
    {
        _quotaRepository = quotaRepository;
        _options = options.Value;
    }

    public async Task<AiGenerationStatusDto> Handle(GetAiGenerationStatusQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var quota = await _quotaRepository.GetByTenantAndDateAsync(request.TenantId, today, ct);

        var usedToday = quota?.GenerationCount ?? 0;
        var canGenerate = usedToday < _options.MaxGenerationsPerDay;

        DateTime? nextAvailableAt = canGenerate
            ? null
            : DateTime.UtcNow.Date.AddDays(1);

        return new AiGenerationStatusDto(canGenerate, usedToday, _options.MaxGenerationsPerDay, nextAvailableAt);
    }
}
