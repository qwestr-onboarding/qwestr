namespace Qwestr.Modules.Onboarding.Application.Options;

public class OnboardingAiOptions
{
    public const string SectionName = "Onboarding:Ai";

    public int MaxGenerationsPerDay { get; set; } = 1;
    public int DefaultMaxStages { get; set; } = 10;
    public int DefaultMaxStepsPerStage { get; set; } = 8;
    /// <summary>
    /// Max characters extracted from a single attached document.
    /// Default: 500 000 (~200K tokens) — fits comfortably in Qwen/Gemini 1M context.
    /// Lower this for YandexGPT which has a much smaller context window.
    /// </summary>
    public int MaxCharsPerDocument { get; set; } = 500_000;
}
