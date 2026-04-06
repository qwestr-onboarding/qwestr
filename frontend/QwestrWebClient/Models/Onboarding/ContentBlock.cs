using System.Text.Json.Serialization;

namespace QwestrWebClient.Models.Onboarding;

public class ContentBlock
{
    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = ContentBlockType.Text;

    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; set; }

    /// <summary>Для heading: "h1" | "h2" | "h3"</summary>
    [JsonPropertyName("level")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Level { get; set; }

    /// <summary>Для callout: "information" | "warning" | "danger" | "success"</summary>
    [JsonPropertyName("variant")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Variant { get; set; }
}

public static class ContentBlockType
{
    public const string Text     = "text";
    public const string Heading  = "heading";
    public const string Image    = "image";
    public const string Video    = "video";
    public const string Divider  = "divider";
    public const string Callout  = "callout";
}