using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Onboarding.Api.Endpoints;

public static class OnboardingEndpoints
{
    public static IEndpointRouteBuilder MapOnboardingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/onboarding").WithTags("Onboarding");
        group.MapGet("/scenarios", GetScenarios);
        
        return app;
    }
    
    private static async Task<IResult> GetScenarios(CancellationToken ct)
    {
        // var result = await service.GetScenariosAsync(ct);
        return Results.Ok();
    }
}