using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Qwestr.Modules.Notifications.Application.Abstractions;

namespace Qwestr.Modules.Notifications.Infrastructure.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;

    public SmtpEmailSender(IOptions<EmailOptions> options) => _options = options.Value;

    public async Task SendAsync(
        string toAddress,
        string? toDisplayName,
        string subject,
        string bodyHtml,
        string? bodyText,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.From.DisplayName, _options.From.Address));
        message.To.Add(new MailboxAddress(toDisplayName ?? toAddress, toAddress));

        if (!string.IsNullOrWhiteSpace(_options.ReplyTo))
        {
            message.ReplyTo.Add(new MailboxAddress("Support", _options.ReplyTo));
        }

        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = bodyHtml };
        if (!string.IsNullOrWhiteSpace(bodyText))
        {
            bodyBuilder.TextBody = bodyText;
        }
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        var socketOptions = _options.Smtp.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTlsWhenAvailable;

        await client.ConnectAsync(_options.Smtp.Host, _options.Smtp.Port, socketOptions, cancellationToken);
        await client.AuthenticateAsync(_options.Smtp.Username, _options.Smtp.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
