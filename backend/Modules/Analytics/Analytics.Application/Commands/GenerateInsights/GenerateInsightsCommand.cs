using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Analytics.Responses;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Analytics.Application.Commands.GenerateInsights;

public record GenerateInsightsCommand(Guid TenantId, LlmProvider Provider)
    : IRequest<ActionResult<AnalyticsInsightSetResponse>>;
