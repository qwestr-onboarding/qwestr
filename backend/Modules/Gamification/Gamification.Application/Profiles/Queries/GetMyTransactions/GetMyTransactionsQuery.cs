using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyTransactions;

public record GetMyTransactionsQuery(
    Guid TenantId,
    Guid UserId,
    DateTime? From,
    DateTime? To,
    int Limit) : IRequest<ActionResult<PointsTransactionListResponse>>;
