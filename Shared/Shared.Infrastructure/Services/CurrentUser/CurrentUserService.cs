using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Qwestr.Shared.Infrastructure.Services.CurrentUser;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CurrentUserService(IHttpContextAccessor contextAccessor)
        => _contextAccessor = contextAccessor;

    private ClaimsPrincipal User =>
        _contextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("No HttpContext");

    public Guid UserId =>
        GetGuidClaim(ClaimTypes.NameIdentifier);

    public Guid TenantId =>
        GetGuidClaim("tenant_id");
    
    public IReadOnlyCollection<string> Roles =>
        User.FindAll(ClaimTypes.Role)
            .Select(r => r.Value)
            .ToList();

    private Guid GetGuidClaim(string type)
    {
        var value = User.FindFirst(type)?.Value;

        if (!Guid.TryParse(value, out var guid))
            throw new UnauthorizedAccessException($"Invalid claim: {type}");

        return guid;
    }
}