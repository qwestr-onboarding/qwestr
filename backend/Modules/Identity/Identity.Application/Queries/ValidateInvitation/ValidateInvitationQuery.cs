using MediatR;
using Qwestr.Shared.Contracts.ApiDto.Identity.Invitations.Responses;
using Qwestr.Shared.Kernel.Entities;

namespace Qwestr.Modules.Identity.Application.Queries.ValidateInvitation;

public record ValidateInvitationQuery(string Token) : IRequest<ActionResult<InvitationPreviewResponse>>;