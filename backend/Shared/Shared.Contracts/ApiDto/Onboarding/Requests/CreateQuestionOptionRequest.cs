namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record CreateQuestionOptionRequest(
    Guid QuestionId,
    string Text,
    bool IsCorrect);
