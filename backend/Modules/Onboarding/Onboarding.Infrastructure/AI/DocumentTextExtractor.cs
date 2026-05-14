using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Qwestr.Modules.Onboarding.Application.AI.Models;
using Qwestr.Modules.Onboarding.Application.Options;
using Serilog;
using UglyToad.PdfPig;

namespace Qwestr.Modules.Onboarding.Infrastructure.AI;

public class DocumentTextExtractor
{
    private readonly int _maxChars;
    private readonly ILogger _logger = Log.ForContext<DocumentTextExtractor>();

    public DocumentTextExtractor(IOptions<OnboardingAiOptions> options)
    {
        _maxChars = options.Value.MaxCharsPerDocument;
    }

    public string? Extract(AttachedDocument doc)
    {
        try
        {
            var bytes = Convert.FromBase64String(doc.ContentBase64);
            var text = doc.MimeType switch
            {
                "text/plain" => Encoding.UTF8.GetString(bytes),
                "application/pdf" => ExtractPdf(bytes),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ExtractDocx(bytes),
                _ => null
            };

            if (text is null) return null;

            text = text.Trim();
            if (text.Length > _maxChars)
                text = text[.._maxChars] + "\n[... текст обрезан ...]";

            return text;
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Failed to extract text from document {FileName}", doc.FileName);
            return null;
        }
    }

    private static string ExtractPdf(byte[] bytes)
    {
        using var doc = PdfDocument.Open(bytes);
        var sb = new StringBuilder();
        foreach (var page in doc.GetPages())
        {
            foreach (var word in page.GetWords())
                sb.Append(word.Text).Append(' ');
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string ExtractDocx(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
        var entry = zip.GetEntry("word/document.xml");
        if (entry is null) return string.Empty;

        using var stream = entry.Open();
        var xmlDoc = XDocument.Load(stream);
        XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        var sb = new StringBuilder();
        foreach (var paragraph in xmlDoc.Descendants(w + "p"))
        {
            var line = string.Concat(paragraph.Descendants(w + "t").Select(e => e.Value));
            if (!string.IsNullOrWhiteSpace(line))
                sb.AppendLine(line);
        }
        return sb.ToString();
    }
}
