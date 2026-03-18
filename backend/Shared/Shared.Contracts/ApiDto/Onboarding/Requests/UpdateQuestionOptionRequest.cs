namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record UpdateQuestionOptionRequest(
    string Text,
    bool IsCorrect);
