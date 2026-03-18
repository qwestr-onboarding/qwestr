using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record CreateQuestionRequest(
    Guid StepId,
    QuestionType Type,
    string QuestionText);
