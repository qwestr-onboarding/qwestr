namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Requests;

public record CreateManualAwardRequest(
    string Code,
    string Title,
    string Description,
    string IconUrl,
    int XpBonus,
    int CoinsBonus);
