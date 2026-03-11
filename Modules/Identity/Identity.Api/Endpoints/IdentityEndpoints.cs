using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Qwestr.Modules.Identity.Api.Endpoints;

public static partial class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var identity = app.MapGroup("/api/identity");
        identity.MapAuthEndpoints();
        return app;
    }
}
