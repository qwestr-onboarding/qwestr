using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetTestAttemptResult;

public record GetTestAttemptResultQuery(
    Guid TenantId,
    Guid UserId,
    Guid TestAttemptId) : IRequest<ActionResult<TestResultResponse>>;
