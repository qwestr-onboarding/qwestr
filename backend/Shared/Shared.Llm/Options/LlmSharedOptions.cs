using Qwestr.Shared.Kernel.Enums.Analytics;

namespace Qwestr.Shared.Llm.Options;

public class LlmSharedOptions
{
    public const string SectionName = "Llm";

    public LlmProvider DefaultProvider { get; set; } = LlmProvider.YandexGpt;
    public bool UseFake { get; set; } = false;
    public YandexGptOptions YandexGpt { get; set; } = new();
    public GeminiOptions Gemini { get; set; } = new();
    public QwenOptions Qwen { get; set; } = new();
}

public class YandexGptOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string FolderId { get; set; } = string.Empty;
    public string ModelUri { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0.2;
    public int MaxTokens { get; set; } = 4000;
}

public class GeminiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
    public double Temperature { get; set; } = 0.2;
}

public class QwenOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://dashscope-intl.aliyuncs.com/compatible-mode/v1";
    public string Model { get; set; } = "qwen-plus";
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 4000;
    public int TimeoutSeconds { get; set; } = 240;
}
