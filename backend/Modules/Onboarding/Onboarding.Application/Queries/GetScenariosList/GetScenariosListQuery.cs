using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Onboarding.Application.Queries.GetScenariosList;

public record GetScenariosListQuery(
    Guid TenantId,
    int Page,
    int PageSize,
    string? TagFilter) : IRequest<ActionResult<PagedList<ScenarioListItemResponse>>>;
