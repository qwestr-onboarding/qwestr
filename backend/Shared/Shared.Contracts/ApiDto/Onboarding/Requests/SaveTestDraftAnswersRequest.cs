namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record SaveTestDraftAnswerItem(
    Guid QuestionId,
    List<Guid>? SelectedOptionIds,
    string? TextAnswer);

public record SaveTestDraftAnswersRequest(List<SaveTestDraftAnswerItem> Answers);
