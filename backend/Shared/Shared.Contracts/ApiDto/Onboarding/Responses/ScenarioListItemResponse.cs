namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record ScenarioListItemResponse(
    Guid Id,
    string Title,
    string? Description,
    List<string> Tags,
    bool IsPublished,
    int StageCount,
    int StepCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
