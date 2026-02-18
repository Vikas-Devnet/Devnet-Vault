namespace Application.Features.Common.Models;

public class EmailSettings
{
    public string IssuerEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
}
