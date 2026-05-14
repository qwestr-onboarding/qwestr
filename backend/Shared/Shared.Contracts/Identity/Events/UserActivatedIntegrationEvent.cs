using MediatR;

namespace Qwestr.Shared.Contracts.Identity.Events;

public record UserActivatedIntegrationEvent(
    Guid TenantId,
    string TenantSubdomain,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string CompanyName) : INotification;
