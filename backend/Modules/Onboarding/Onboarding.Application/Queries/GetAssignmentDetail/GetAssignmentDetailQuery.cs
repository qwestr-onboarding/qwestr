using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetAssignmentDetail;

public record GetAssignmentDetailQuery(
    Guid TenantId,
    Guid AssignmentId) : IRequest<ActionResult<AssignmentDetailResponse>>;
