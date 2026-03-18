using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetMyAssignmentProgress;

public record GetMyAssignmentProgressQuery(
    Guid TenantId,
    Guid UserId,
    Guid AssignmentId) : IRequest<ActionResult<MyAssignmentProgressResponse>>;
