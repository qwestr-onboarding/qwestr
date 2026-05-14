using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Modules.Onboarding.Application.AI.Models;

public record ScenarioGenerationRequest(
    string Query,
    List<AttachedDocument>? Documents,
    int StageCount,
    int StepsPerStage,
    string TextStyle,
    string? AdditionalInfo,
    string? TargetAudience,
    bool IncludeFinalTest,
    int? TotalPoints,
    LlmProvider? Provider);

public record AttachedDocument(string FileName, string ContentBase64, string MimeType);
