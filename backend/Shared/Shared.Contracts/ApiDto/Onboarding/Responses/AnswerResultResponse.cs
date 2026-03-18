namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;

public record AnswerResultResponse(
    bool IsCorrect,
    List<Guid> CorrectOptionIds);
