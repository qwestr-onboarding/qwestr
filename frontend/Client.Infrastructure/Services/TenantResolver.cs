using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Client.Infrastructure.Services;

public class TenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public readonly string BaseDomain;

    public TenantResolver(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        BaseDomain = configuration["App:BaseDomain"]
            ?? throw new InvalidOperationException("App:BaseDomain is not configured.");
    }

    public string? GetSubdomain()
    {
        var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
        if (string.IsNullOrEmpty(host))
        {
            return null;
        }

        var suffix = $".{BaseDomain}";
        if (!host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return host[..host.IndexOf('.')];
    }
}