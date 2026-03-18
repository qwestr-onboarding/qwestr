using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetMyAssignments;

public record GetMyAssignmentsQuery(
    Guid TenantId,
    Guid UserId) : IRequest<ActionResult<List<MyAssignmentSummaryResponse>>>;
