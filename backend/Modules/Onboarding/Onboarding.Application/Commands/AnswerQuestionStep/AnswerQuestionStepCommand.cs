using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.AnswerQuestionStep;

public record AnswerQuestionStepCommand(
    Guid TenantId,
    Guid UserId,
    Guid StepProgressId,
    Guid QuestionId,
    List<Guid>? SelectedOptionIds,
    string? TextAnswer) : IRequest<ActionResult<AnswerResultResponse>>;
