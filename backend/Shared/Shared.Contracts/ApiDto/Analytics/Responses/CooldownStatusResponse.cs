namespace Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;

public record CooldownStatusResponse(bool IsAvailable, int? RemainingSeconds, DateTime? AvailableAt);
