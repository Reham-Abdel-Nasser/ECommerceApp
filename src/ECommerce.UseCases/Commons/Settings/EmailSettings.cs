namespace ECommerce.UseCases.Commons.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "Email";
    public string FromEmail { get; set; } = null!;
    public string FromName { get; set; } = "ECommerce";
    public string Host { get; set; } = "smtp-relay.brevo.com";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string? Username { get; set; }
    public string? Password { get; set; }
}