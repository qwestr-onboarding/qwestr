namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record SubmitTestAnswerItemRequest(
    Guid QuestionId,
    List<Guid>? SelectedOptionIds,
    string? TextAnswer);

public record SubmitTestAnswersRequest(List<SubmitTestAnswerItemRequest> Answers);
