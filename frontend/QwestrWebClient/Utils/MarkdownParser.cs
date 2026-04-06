using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace QwestrWebClient.Utils;

public static partial class MarkdownParser
{
    /// <summary>
    /// Преобразует строку с Markdown в безопасный HTML (MarkupString).
    /// Поддерживаются: **жирный**, *курсив*, `код`, [текст](url), - списки, 1. нумерованные.
    /// </summary>
    public static MarkupString Render(string? text)
    {
        if (string.IsNullOrEmpty(text)) return new MarkupString(string.Empty);

        var lines = text.ReplaceLineEndings("\n").Split('\n');
        var sb = new StringBuilder();
        var i = 0;

        while (i < lines.Length)
        {
            var line = lines[i];

            // Unordered list — группируем подряд идущие строки
            if (IsUnorderedItem(line))
            {
                sb.Append("<ul style=\"margin:4px 0 8px;padding-left:22px;\">");
                while (i < lines.Length && IsUnorderedItem(lines[i]))
                {
                    sb.Append("<li style=\"line-height:1.7;\">")
                      .Append(RenderInline(lines[i][2..]))
                      .Append("</li>");
                    i++;
                }
                sb.Append("</ul>");
                continue;
            }

            // Ordered list
            if (IsOrderedItem(line))
            {
                sb.Append("<ol style=\"margin:4px 0 8px;padding-left:22px;\">");
                while (i < lines.Length && IsOrderedItem(lines[i]))
                {
                    var content = OrderedItemContent().Match(lines[i]).Groups[1].Value;
                    sb.Append("<li style=\"line-height:1.7;\">")
                      .Append(RenderInline(content))
                      .Append("</li>");
                    i++;
                }
                sb.Append("</ol>");
                continue;
            }

            // Пустая строка → небольшой отступ
            if (string.IsNullOrWhiteSpace(line))
            {
                sb.Append("<div style=\"height:0.5em;\"></div>");
                i++;
                continue;
            }

            // Обычная строка текста
            sb.Append("<span style=\"display:block;line-height:1.7;\">")
              .Append(RenderInline(line))
              .Append("</span>");
            i++;
        }

        return new MarkupString(sb.ToString());
    }

    /// <summary>Только инлайн-элементы (без списков) — для callout-блоков.</summary>
    public static MarkupString RenderInlineOnly(string? text)
    {
        if (string.IsNullOrEmpty(text)) return new MarkupString(string.Empty);
        return new MarkupString(RenderInline(text));
    }

    private static bool IsUnorderedItem(string line) =>
        line.StartsWith("- ") || line.StartsWith("* ");

    private static bool IsOrderedItem(string line) =>
        OrderedItemContent().IsMatch(line);

    private static string RenderInline(string text)
    {
        // HTML-encode сначала — защита от XSS
        text = WebUtility.HtmlEncode(text);

        // `инлайн-код` — обрабатываем до * чтобы звёздочки внутри не матчились
        text = InlineCode().Replace(text,
            "<code style=\"background:rgba(0,0,0,0.07);border-radius:3px;padding:1px 5px;" +
            "font-size:0.875em;font-family:monospace;\">$1</code>");

        // **жирный**
        text = Bold().Replace(text, "<strong>$1</strong>");

        // *курсив* (одиночная звёздочка, не часть **)
        text = Italic().Replace(text, "<em>$1</em>");

        // [текст](url)
        text = Link().Replace(text,
            "<a href=\"$2\" target=\"_blank\" rel=\"noopener noreferrer\" " +
            "style=\"color:var(--mud-palette-primary);text-decoration:underline;\">$1</a>");

        return text;
    }

    [GeneratedRegex(@"`([^`]+)`")]
    private static partial Regex InlineCode();

    [GeneratedRegex(@"\*\*(.+?)\*\*")]
    private static partial Regex Bold();

    [GeneratedRegex(@"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)")]
    private static partial Regex Italic();

    [GeneratedRegex(@"\[([^\]]+)\]\(([^)]+)\)")]
    private static partial Regex Link();

    [GeneratedRegex(@"^\d+\. (.*)")]
    private static partial Regex OrderedItemContent();
}
