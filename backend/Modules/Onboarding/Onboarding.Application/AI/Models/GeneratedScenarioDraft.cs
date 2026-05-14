using Qwestr.Shared.Kernel.Enums.Onboarding;

namespace Qwestr.Modules.Onboarding.Application.AI.Models;

public record GeneratedScenarioDraft(
    string SuggestedTitle,
    string? SuggestedDescription,
    List<GeneratedStageDraft> Stages,
    string AiComment);

public record GeneratedStageDraft(
    string Title,
    string? Description,
    List<GeneratedStepDraft> Steps);

public record GeneratedStepDraft(
    StepType Type,
    string Title,
    int? Points,
    string? ContentJson,
    string? SettingsJson,
    List<GeneratedQuestionDraft>? Questions);

public record GeneratedQuestionDraft(
    QuestionType Type,
    string QuestionText,
    List<GeneratedOptionDraft> Options);

public record GeneratedOptionDraft(string Text, bool IsCorrect);
