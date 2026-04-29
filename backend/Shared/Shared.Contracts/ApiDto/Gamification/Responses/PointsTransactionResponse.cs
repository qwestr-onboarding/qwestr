namespace Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;

public record PointsTransactionResponse(
    Guid Id,
    int XpAmount,
    int CoinsAmount,
    string SourceType,
    string Reason,
    DateTime OccurredAt);

public record PointsTransactionListResponse(List<PointsTransactionResponse> Items);
