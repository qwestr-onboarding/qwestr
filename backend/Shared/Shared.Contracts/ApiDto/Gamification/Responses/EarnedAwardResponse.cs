namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record EarnedAwardResponse(
    Guid Id,
    Guid AwardId,
    string Code,
    string Title,
    string IconUrl,
    DateTime EarnedAt,
    bool IsManual);

public record EarnedAwardListResponse(List<EarnedAwardResponse> Items);
