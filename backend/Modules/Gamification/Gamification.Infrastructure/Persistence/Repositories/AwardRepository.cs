using Microsoft.EntityFrameworkCore;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Modules.Gamification.Domain.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;

namespace Qwestr.Modules.Gamification.Infrastructure.Persistence.Repositories;

public class AwardRepository : IAwardRepository
{
    private readonly GamificationDbContext _context;

    public AwardRepository(GamificationDbContext context)
    {
        _context = context;
    }

    public async Task<Award?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Awards.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<List<Award>> GetByTenantAsync(Guid tenantId, CancellationToken ct = default)
        => await _context.Awards
            .Where(a => a.TenantId == null || a.TenantId == tenantId)
            .ToListAsync(ct);

    public async Task<List<Award>> GetActiveByTenantAndTriggerAsync(Guid tenantId, AwardIssuanceType issuanceType, RuleTrigger trigger, CancellationToken ct = default)
    {
        // Rule хранится как jsonb через value converter — EF Core не может транслировать
        // фильтр по Rule.Trigger в SQL, поэтому фильтруем по plain-колонкам в БД,
        // а Trigger проверяем уже после десериализации на стороне клиента.
        var awards = await _context.Awards
            .Where(a => (a.TenantId == null || a.TenantId == tenantId)
                        && a.IssuanceType == issuanceType
                        && a.IsActive
                        && a.Rule != null)
            .ToListAsync(ct);

        return awards.Where(a => a.Rule!.Trigger == trigger).ToList();
    }

    public async Task<List<EarnedAward>> GetEarnedByProfileAsync(Guid profileId, CancellationToken ct = default)
        => await _context.EarnedAwards
            .Where(e => e.ProfileId == profileId)
            .OrderByDescending(e => e.EarnedAt)
            .ToListAsync(ct);

    public async Task<bool> IsAwardAlreadyEarnedAsync(Guid profileId, Guid awardId, CancellationToken ct = default)
        => await _context.EarnedAwards.AnyAsync(e => e.ProfileId == profileId && e.AwardId == awardId, ct);

    public async Task<AchievementProgress?> GetProgressAsync(Guid profileId, Guid awardId, CancellationToken ct = default)
        => await _context.AchievementProgresses.FirstOrDefaultAsync(p => p.ProfileId == profileId && p.AwardId == awardId, ct);

    public async Task<List<AchievementProgress>> GetAllProgressByProfileAsync(Guid profileId, CancellationToken ct = default)
        => await _context.AchievementProgresses.Where(p => p.ProfileId == profileId).ToListAsync(ct);

    public async Task AddAsync(Award award, CancellationToken ct = default)
        => await _context.Awards.AddAsync(award, ct);

    public async Task AddEarnedAwardAsync(EarnedAward earnedAward, CancellationToken ct = default)
        => await _context.EarnedAwards.AddAsync(earnedAward, ct);

    public async Task AddProgressAsync(AchievementProgress progress, CancellationToken ct = default)
        => await _context.AchievementProgresses.AddAsync(progress, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
