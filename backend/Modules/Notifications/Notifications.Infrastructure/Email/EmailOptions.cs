namespace Qwestr.Modules.Notifications.Infrastructure.Email;

public sealed class EmailOptions
{
    public SmtpOptions Smtp { get; set; } = new();
    public FromOptions From { get; set; } = new();
    public string? ReplyTo { get; set; }
    public string BaseUrl { get; set; } = "https://app.qwestr.ru";
}

public sealed class SmtpOptions
{
    public string Host { get; set; } = "smtp.mail.ru";
    public int Port { get; set; } = 465;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public sealed class FromOptions
{
    public string Address { get; set; } = "notifications@qwestr.ru";
    public string DisplayName { get; set; } = "Qwestr";
}
