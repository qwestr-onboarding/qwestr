using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Qwestr.Modules.Onboarding.Application.AI.Models;
using Qwestr.Modules.Onboarding.Application.Commands.GenerateOnboardingScenario;
using Qwestr.Modules.Onboarding.Application.Queries.GetAiGenerationStatus;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Requests;
using Qwestr.Shared.Contracts.ApiDto.Onboarding.Responses;
using Qwestr.Shared.Contracts.ApiDto.Shared.Responses;
using Qwestr.Shared.Infrastructure.Services.CurrentUser;

namespace Qwestr.Modules.Onboarding.Api.Endpoints;

public static partial class OnboardingEndpoints
{
    private static IEndpointRouteBuilder MapAiEndpoints(this IEndpointRouteBuilder onboarding)
    {
        var ai = onboarding.MapGroup("ai").WithTags("Onboarding.AI").RequireAuthorization(HrPolicy);
        var scenarios = onboarding.MapGroup("scenarios").WithTags("Onboarding.Scenarios").RequireAuthorization(HrPolicy);

        ai.MapGet("/status", GetAiStatusHandler);
        scenarios.MapPost("/generate", GenerateScenarioHandler);

        return onboarding;
    }

    private static async Task<IResult> GetAiStatusHandler(
        ICurrentUserService currentUser, ISender sender, CancellationToken ct)
    {
        var dto = await sender.Send(new GetAiGenerationStatusQuery(currentUser.TenantId), ct);
        return Results.Ok(new AiGenerationStatusResponse(
            dto.CanGenerate, dto.UsedToday, dto.MaxPerDay, dto.NextAvailableAt));
    }

    private static async Task<IResult> GenerateScenarioHandler(
        GenerateOnboardingScenarioRequest body, ICurrentUserService currentUser, ISender sender)
    {
        // CancellationToken intentionally omitted: browser disconnect or proxy timeout would cancel
        // the ASP.NET request ct and abort the long-running Qwen call mid-flight (SocketException 995).
        // Generation runs to completion regardless of client state; the scenario is saved on success.
        var result = await sender.Send(new GenerateOnboardingScenarioCommand(
            currentUser.TenantId,
            currentUser.UserId,
            body.Query,
            body.Documents?.Select(d => new AttachedDocument(d.FileName, d.ContentBase64, d.MimeType)).ToList(),
            body.StageCount,
            body.StepsPerStage,
            body.TextStyle,
            body.AdditionalInfo,
            body.TargetAudience,
            body.IncludeFinalTest,
            body.TotalPoints,
            body.Provider), CancellationToken.None);

        return result.IsSuccess
            ? Results.Created(
                $"/api/onboarding/scenarios/{result.Value!.ScenarioId}",
                new GenerateOnboardingScenarioResponse(result.Value.ScenarioId, result.Value.AiComment))
            : Results.BadRequest(new ErrorResponse(result.Error));
    }
}
