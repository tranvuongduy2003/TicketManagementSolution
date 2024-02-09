using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Services;

public class SendService : ISendService
{
    private readonly IConfiguration _configuration;
    private readonly EmailSettings emailSettings;

    public SendService(IOptions<EmailSettings> options,IConfiguration configuration)
    {
        this.emailSettings = options.Value;
        _configuration = configuration;
    }
    
    public async Task SendEmail(string destinationEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(emailSettings.Email));
        email.To.Add(MailboxAddress.Parse(destinationEmail));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(emailSettings.Email, emailSettings.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}