namespace Qwestr.Modules.Notifications.Application.Models;

public sealed record RenderedEmail(string Subject, string BodyHtml, string? BodyText);
