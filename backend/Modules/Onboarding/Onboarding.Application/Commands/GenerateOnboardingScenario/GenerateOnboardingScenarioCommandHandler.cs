using MediatR;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Onboarding.Application.Abstractions.Repositories;
using Qwestr.Modules.Onboarding.Application.AI.Abstractions;
using Qwestr.Modules.Onboarding.Application.AI.Models;
using Qwestr.Modules.Onboarding.Application.Options;
using Qwestr.Modules.Onboarding.Domain.Entities;
using Qwestr.Shared.Kernel.Entities;
using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.Commands.GenerateOnboardingScenario;

public class GenerateOnboardingScenarioCommandHandler
    : IRequestHandler<GenerateOnboardingScenarioCommand, ActionResult<GenerateOnboardingScenarioResult>>
{
    private readonly IOnboardingScenarioGenerator _generator;
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IAiGenerationQuotaRepository _quotaRepository;
    private readonly OnboardingAiOptions _options;

    public GenerateOnboardingScenarioCommandHandler(
        IOnboardingScenarioGenerator generator,
        IScenarioRepository scenarioRepository,
        IAiGenerationQuotaRepository quotaRepository,
        IOptions<OnboardingAiOptions> options)
    {
        _generator = generator;
        _scenarioRepository = scenarioRepository;
        _quotaRepository = quotaRepository;
        _options = options.Value;
    }

    public async Task<ActionResult<GenerateOnboardingScenarioResult>> Handle(
        GenerateOnboardingScenarioCommand request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var quota = await _quotaRepository.GetByTenantAndDateAsync(request.TenantId, today, ct);

        if (quota is not null && !quota.CanGenerate(_options.MaxGenerationsPerDay))
            return ActionResult<GenerateOnboardingScenarioResult>.Failure(
                "onboarding.ai.daily_limit_reached");

        var generationRequest = new ScenarioGenerationRequest(
            request.Query,
            request.Documents,
            request.StageCount,
            request.StepsPerStage,
            request.TextStyle,
            request.AdditionalInfo,
            request.TargetAudience,
            request.IncludeFinalTest,
            request.TotalPoints,
            request.Provider);

        GeneratedScenarioDraft draft;
        try
        {
            draft = await _generator.GenerateAsync(generationRequest, ct);
        }
        catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
        {
            return ActionResult<GenerateOnboardingScenarioResult>.Failure("onboarding.ai.timeout");
        }
        catch (Exception)
        {
            return ActionResult<GenerateOnboardingScenarioResult>.Failure("onboarding.ai.generation_failed");
        }

        var scenario = Scenario.Create(request.TenantId, draft.SuggestedTitle, draft.SuggestedDescription, []);
        await _scenarioRepository.AddAsync(scenario, ct);

        for (var stageIndex = 0; stageIndex < draft.Stages.Count; stageIndex++)
        {
            var stageDraft = draft.Stages[stageIndex];
            var stage = Stage.Create(scenario.Id, stageDraft.Title, stageDraft.Description, stageIndex + 1);
            await _scenarioRepository.AddStageAsync(stage, ct);

            for (var stepIndex = 0; stepIndex < stageDraft.Steps.Count; stepIndex++)
            {
                var stepDraft = stageDraft.Steps[stepIndex];
                var step = Step.Create(
                    stage.Id,
                    stepDraft.Title,
                    stepDraft.Type,
                    stepIndex + 1,
                    stepDraft.Points,
                    stepDraft.ContentJson,
                    stepDraft.SettingsJson);
                await _scenarioRepository.AddStepAsync(step, ct);

                if (stepDraft.Questions is { Count: > 0 })
                {
                    for (var qIndex = 0; qIndex < stepDraft.Questions.Count; qIndex++)
                    {
                        var qDraft = stepDraft.Questions[qIndex];
                        var question = Question.Create(step.Id, qDraft.Type, qDraft.QuestionText, qIndex + 1);
                        await _scenarioRepository.AddQuestionAsync(question, ct);

                        for (var oIndex = 0; oIndex < qDraft.Options.Count; oIndex++)
                        {
                            var oDraft = qDraft.Options[oIndex];
                            var option = QuestionOption.Create(question.Id, oDraft.Text, oDraft.IsCorrect, oIndex + 1);
                            await _scenarioRepository.AddOptionAsync(option, ct);
                        }
                    }
                }
            }
        }

        if (quota is null)
        {
            quota = AiGenerationQuota.Create(request.TenantId, today);
            await _quotaRepository.AddAsync(quota, ct);
        }
        quota.Increment();

        await _scenarioRepository.SaveChangesAsync(ct);
        await _quotaRepository.SaveChangesAsync(ct);

        return ActionResult<GenerateOnboardingScenarioResult>.Success(
            new GenerateOnboardingScenarioResult(scenario.Id, draft.AiComment));
    }
}
