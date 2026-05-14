using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Domain.Entities;

public sealed class AiGenerationQuota : Entity<Guid>
{
    private AiGenerationQuota() { }

    public Guid TenantId { get; private set; }
    public DateOnly Date { get; private set; }
    public int GenerationCount { get; private set; }

    public static AiGenerationQuota Create(Guid tenantId, DateOnly date)
    {
        return new AiGenerationQuota
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Date = date,
            GenerationCount = 0
        };
    }

    public bool CanGenerate(int maxPerDay) => GenerationCount < maxPerDay;

    public void Increment() => GenerationCount++;
}
