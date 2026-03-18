using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetAssignmentsList;

public record GetAssignmentsListQuery(
    Guid TenantId,
    int Page,
    int PageSize,
    Guid? ScenarioId,
    Guid? UserId) : IRequest<ActionResult<PagedList<AssignmentListItemResponse>>>;
