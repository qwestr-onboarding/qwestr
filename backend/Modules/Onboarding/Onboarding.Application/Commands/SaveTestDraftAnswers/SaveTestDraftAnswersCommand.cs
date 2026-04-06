using MediatR;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Commands.SaveTestDraftAnswers;

public record DraftAnswerDto(
    Guid QuestionId,
    List<Guid>? SelectedOptionIds,
    string? TextAnswer);

public record SaveTestDraftAnswersCommand(
    Guid TenantId,
    Guid UserId,
    Guid TestAttemptId,
    List<DraftAnswerDto> Answers) : IRequest<ActionResult>;
