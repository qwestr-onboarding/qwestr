using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;

public record UpdateQuestionRequest(
    QuestionType Type,
    string QuestionText);
