namespace Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;

public record EngagementTrendResponse(IReadOnlyList<EngagementPointResponse> Points);
