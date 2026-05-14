using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    internal const string HrPolicy = "OnboardingHrOnly";

    public static IEndpointRouteBuilder MapOnboardingEndpoints(this IEndpointRouteBuilder app)
    {
        var onboarding = app.MapGroup("/api/onboarding");
        onboarding.MapScenariosEndpoints();
        onboarding.MapStagesEndpoints();
        onboarding.MapStepsEndpoints();
        onboarding.MapQuestionsEndpoints();
        onboarding.MapOptionsEndpoints();
        onboarding.MapAssignmentsEndpoints();
        onboarding.MapProgressEndpoints();
        onboarding.MapAiEndpoints();
        return app;
    }
}
