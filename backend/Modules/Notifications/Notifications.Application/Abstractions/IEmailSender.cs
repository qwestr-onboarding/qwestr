namespace Qwestr.Modules.Notifications.Application.Abstractions;

public interface IEmailSender
{
    Task SendAsync(
        string toAddress,
        string? toDisplayName,
        string subject,
        string bodyHtml,
        string? bodyText,
        CancellationToken cancellationToken = default);
}
