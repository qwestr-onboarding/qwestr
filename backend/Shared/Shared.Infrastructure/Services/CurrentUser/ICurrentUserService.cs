namespace Qwestr.Shared.Infrastructure.Services.CurrentUser;

public interface ICurrentUserService
{
    Guid UserId { get; }
    
    Guid TenantId { get; }
    
    IReadOnlyCollection<string> Roles { get; }
}