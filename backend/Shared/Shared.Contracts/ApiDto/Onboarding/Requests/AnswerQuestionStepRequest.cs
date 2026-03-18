namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record AnswerQuestionStepRequest(
    Guid QuestionId,
    List<Guid>? SelectedOptionIds,
    string? TextAnswer);
