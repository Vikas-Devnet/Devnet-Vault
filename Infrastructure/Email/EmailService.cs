using Application.Features.Common.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Email;

public class EmailService(IOptions<EmailSettings> _emailOptions)
{
    private readonly EmailSettings _emailSettings = _emailOptions.Value;
    public async Task SendAsync(EmailMessage message)
    {
        using var smtp = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
        {
            Credentials = new NetworkCredential(_emailSettings.IssuerEmail, _emailSettings.Password),
            EnableSsl = true
        };

        var mail = new MailMessage(
           _emailSettings.IssuerEmail,
            message.To,
            message.Subject,
            message.Body)
        {
            IsBodyHtml = message.IsHtml
        };

        await smtp.SendMailAsync(mail);
    }
}
