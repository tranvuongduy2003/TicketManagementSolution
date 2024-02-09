using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Contracts;

public interface IEmailService
{
    Task RegisterUserEmailAndLog(string email);
    Task ValidateTicketEmailAndLog(ValidateStripeResponseDto ticket);
}