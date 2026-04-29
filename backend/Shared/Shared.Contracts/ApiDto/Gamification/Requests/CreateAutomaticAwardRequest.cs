namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;

public record CreateAutomaticAwardRequest(
    string Code,
    string Title,
    string Description,
    string IconUrl,
    string Trigger,
    string Condition,
    int TargetValue,
    Dictionary<string, string>? FilterParams,
    int XpBonus,
    int CoinsBonus);
