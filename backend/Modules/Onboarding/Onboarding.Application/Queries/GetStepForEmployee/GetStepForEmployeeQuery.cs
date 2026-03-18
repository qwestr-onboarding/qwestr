using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetStepForEmployee;

public record GetStepForEmployeeQuery(
    Guid TenantId,
    Guid UserId,
    Guid AssignmentId,
    Guid StepId) : IRequest<ActionResult<StepContentResponse>>;
