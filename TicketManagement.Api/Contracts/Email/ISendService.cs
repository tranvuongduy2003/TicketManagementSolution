namespace TicketManagement.Api.Contracts;

public interface ISendService
{
    Task SendEmail(string destinationEmail, string subject, string body);
}