using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.SubmitTestAnswers;

public record TestQuestionAnswerDto(
    Guid QuestionId,
    List<Guid>? SelectedOptionIds,
    string? TextAnswer);

public record SubmitTestAnswersCommand(
    Guid TenantId,
    Guid UserId,
    Guid TestAttemptId,
    List<TestQuestionAnswerDto> Answers) : IRequest<ActionResult<TestResultResponse>>;
