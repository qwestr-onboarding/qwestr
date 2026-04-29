using MediatR;
using Qwestr.Modules.Gamification.Application.Abstractions.Persistence;
using Qwestr.Shared.Contracts.ApiDto.Gamification.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Gamification.Application.Profiles.Queries.GetMyTransactions;

public class GetMyTransactionsQueryHandler : IRequestHandler<GetMyTransactionsQuery, ActionResult<PointsTransactionListResponse>>
{
    private readonly IPlayerProfileRepository _profileRepository;
    private readonly IPointsTransactionRepository _transactionRepository;

    public GetMyTransactionsQueryHandler(
        IPlayerProfileRepository profileRepository,
        IPointsTransactionRepository transactionRepository)
    {
        _profileRepository = profileRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<ActionResult<PointsTransactionListResponse>> Handle(GetMyTransactionsQuery request, CancellationToken ct)
    {
        var profile = await _profileRepository.GetByUserIdAsync(request.TenantId, request.UserId, ct);
        if (profile == null)
        {
            return ActionResult<PointsTransactionListResponse>.Failure("Profile.NotFound");
        }

        var transactions = await _transactionRepository.GetByProfileAsync(profile.Id, request.From, request.To, request.Limit, ct);

        var result = transactions.Select(t => new PointsTransactionResponse(
            t.Id,
            t.XpAmount,
            t.CoinsAmount,
            t.SourceType.ToString(),
            t.Reason,
            t.OccurredAt)).ToList();

        return ActionResult<PointsTransactionListResponse>.Success(new PointsTransactionListResponse(result));
    }
}
