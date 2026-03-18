using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record StepContentResponse(
    Guid StepId,
    Guid StepProgressId,
    string Title,
    StepType Type,
    int? Points,
    string? Content,
    string? Settings,
    StepProgressStatus ProgressStatus,
    List<QuestionContentResponse> Questions);

public record QuestionContentResponse(
    Guid Id,
    QuestionType Type,
    string QuestionText,
    int Order,
    List<EmployeeOptionResponse> Options);

/// <summary>
/// Вариант ответа для сотрудника — без поля IsCorrect.
/// </summary>
public record EmployeeOptionResponse(
    Guid Id,
    string Text,
    int Order);
