using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record ScenarioDetailResponse(
    Guid Id,
    string Title,
    string? Description,
    List<string> Tags,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<StageResponse> Stages);

public record StageResponse(
    Guid Id,
    string Title,
    string? Description,
    int Order,
    List<StepResponse> Steps);

public record StepResponse(
    Guid Id,
    string Title,
    StepType Type,
    int Order,
    int? Points,
    string? Content,
    string? Settings,
    List<QuestionResponse> Questions);

public record QuestionResponse(
    Guid Id,
    QuestionType Type,
    string QuestionText,
    int Order,
    List<QuestionOptionResponse> Options);

public record QuestionOptionResponse(
    Guid Id,
    string Text,
    bool IsCorrect,
    int Order);
