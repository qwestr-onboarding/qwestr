namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record AwardResponse(
    Guid Id,
    Guid? TenantId,
    string Code,
    string Title,
    string Description,
    string IconUrl,
    string IssuanceType,
    AwardRuleResponse? Rule,
    int XpBonus,
    int CoinsBonus,
    bool IsActive);

public record AwardRuleResponse(
    string Trigger,
    string Condition,
    int TargetValue,
    Dictionary<string, string> FilterParams);

public record AwardListResponse(List<AwardResponse> Items);
