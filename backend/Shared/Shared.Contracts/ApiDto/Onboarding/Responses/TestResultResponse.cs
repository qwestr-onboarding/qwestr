namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record TestResultResponse(
    Guid TestAttemptId,
    int Score,
    bool IsPassed,
    int AttemptNumber,
    DateTime CompletedAt,
    List<TestQuestionResultResponse> QuestionResults);

public record TestQuestionResultResponse(
    Guid QuestionId,
    string QuestionText,
    bool IsCorrect,
    List<Guid> SelectedOptionIds,
    List<Guid> CorrectOptionIds);
