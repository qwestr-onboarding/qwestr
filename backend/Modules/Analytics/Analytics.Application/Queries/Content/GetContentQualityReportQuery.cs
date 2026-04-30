using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Analytics.Application.Queries.Content;

public record GetContentQualityReportQuery(Guid TenantId, Guid ScenarioId)
    : IRequest<ActionResult<ContentQualityReportResponse>>;
