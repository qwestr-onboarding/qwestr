using MediatR;
using Microsoft.Extensions.Logging;
using Qwestr.Modules.Identity.Domain.Events;

namespace Qwestr.Modules.Identity.Application.EventHandlers;

public class InvitationCreatedEventHandler : INotificationHandler<InvitationCreatedEvent>
{
    private readonly ILogger<InvitationCreatedEventHandler> _logger;

    public InvitationCreatedEventHandler(ILogger<InvitationCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(InvitationCreatedEvent notification, CancellationToken ct)
    {
        _logger.LogInformation(
            "[DEV] Invitation created for {Email} (TenantId: {TenantId}). Token: {Token}",
            notification.Email,
            notification.TenantId,
            notification.Token);

        return Task.CompletedTask;
    }
}