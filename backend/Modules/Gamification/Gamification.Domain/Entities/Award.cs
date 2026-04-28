using Qwestr.Modules.Gamification.Domain.ValueObjects;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Gamification;
using Qwestr.Shared.Kernel.Interfaces;

namespace Qwestr.Modules.Gamification.Domain.Entities;

public sealed class Award : AggregateRoot<Guid>, IAuditableEntity
{
    public Guid? TenantId { get; private set; }  // null = системный
    public string Code { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string IconUrl { get; private set; } = string.Empty;
    public AwardIssuanceType IssuanceType { get; private set; }
    public AwardRule? Rule { get; private set; }  // только для Automatic
    public int XpBonus { get; private set; }
    public int CoinsBonus { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private Award() { }

    public static ActionResult<Award> CreateAutomatic(
        Guid? tenantId, string code, string title, string description, string iconUrl,
        AwardRule rule, int xpBonus, int coinsBonus)
    {
        var validation = ValidateBasics(code, title, iconUrl);
        if (validation.IsFailure)
        {
            return ActionResult<Award>.Failure(validation.Error);
        }
        if (xpBonus < 0 || coinsBonus < 0)
        {
            return ActionResult<Award>.Failure("Award.NegativeBonus");
        }
        return ActionResult<Award>.Success(new Award
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Code = code, Title = title, Description = description, IconUrl = iconUrl,
            IssuanceType = AwardIssuanceType.Automatic,
            Rule = rule,
            XpBonus = xpBonus,
            CoinsBonus = coinsBonus,
            IsActive = true
        });
    }

    public static ActionResult<Award> CreateManual(
        Guid tenantId, string code, string title, string description, string iconUrl)
    {
        var validation = ValidateBasics(code, title, iconUrl);
        if (validation.IsFailure)
        {
            return ActionResult<Award>.Failure(validation.Error);
        }
        return ActionResult<Award>.Success(new Award
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Code = code, Title = title, Description = description, IconUrl = iconUrl,
            IssuanceType = AwardIssuanceType.Manual,
            Rule = null,
            XpBonus = 0,
            CoinsBonus = 0,
            IsActive = true
        });
    }

    private static ActionResult ValidateBasics(string code, string title, string iconUrl)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return ActionResult.Failure("Award.EmptyCode");
        }
        if (string.IsNullOrWhiteSpace(title))
        {
            return ActionResult.Failure("Award.EmptyTitle");
        }
        if (string.IsNullOrWhiteSpace(iconUrl))
        {
            return ActionResult.Failure("Award.EmptyIcon");
        }
        return ActionResult.Success();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}