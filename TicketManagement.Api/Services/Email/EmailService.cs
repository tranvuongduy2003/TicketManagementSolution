using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Services;

public class EmailService : IEmailService
{
    private DbContextOptions<AppDbContext> _dbOptions;
    private readonly IConfigurationManager _configurationManager;
    private readonly IConfiguration _configuration;
    private EmailSettings emailSettings;

    public EmailService(DbContextOptions<AppDbContext> dbOptions, IConfigurationManager configurationManager)
    {
        _dbOptions = dbOptions;
        emailSettings = configurationManager.GetSection("EmailSettings").Get<EmailSettings>();
    }

    public async Task RegisterUserEmailAndLog(string email)
    {
        string message = "Chúc mừng bạn đã đăng ký tài khoản thành công. <br/> Email : " + email;
        await LogAndEmail(email, "Đăng ký tài khoản mới thành công", message);
    }

    public async Task ValidateTicketEmailAndLog(ValidateStripeResponseDto ticket)
    {
        StringBuilder message = new StringBuilder();

        message.AppendLine("<br/><h2>Giao dịch thành công</h2>");
        message.AppendLine("<br/><h3>Tổng tiền: " + ticket.TotalPrice + "</h3>");
        message.AppendLine(
            $"<br/><h3>Thông tin người mua: {ticket.CustomerName} - {ticket.CustomerEmail} - {ticket.CustomerPhone}</h3>");
        message.Append("<br/>");
        message.Append(
            $"<h3>Thông tin vé: {ticket.Quantity} vé x {((long)(ticket.TotalPrice) * 100) / (ticket.Discount * ticket.Quantity)}</h3>");
        message.Append("<br/>");
        message.Append("<ul>");
        foreach (var item in ticket.Tickets)
        {
            message.Append("<li>");
            message.Append($"Mã vé: <b>{item.TicketCode}</b>");
            message.Append("<br/>");
            message.Append($"{item.OwnerName} - {item.OwnerEmail} - {item.OwnerPhone}");
            message.Append("</li>");
            message.Append("<br/>");
        }
        message.Append("</ul>");
        
        message.Append("<br/>");

        await LogAndEmail(ticket.CustomerEmail, "Successful Payment!", message.ToString());
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
        smtp.Send(email);
        smtp.Disconnect(true);
    }

    private async Task<bool> LogAndEmail(string email, string title, string message)
    {
        try
        {
            EmailLogger emailLog = new()
            {
                Email = email,
                EmailSent = DateTime.Now,
                Message = message
            };
            await using var _db = new AppDbContext(_dbOptions);
            await _db.EmailLoggers.AddAsync(emailLog);
            await _db.SaveChangesAsync();

            await this.SendEmail(email, title, message);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}