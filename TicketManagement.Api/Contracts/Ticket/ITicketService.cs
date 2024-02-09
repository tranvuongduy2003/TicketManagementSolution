using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Contracts;

public interface ITicketService
{
    Task<ListTicketObject> GetTickets(PaginationFilter filter);
    Task<ListTicketObject> GetTicketsByUserId(string userId, PaginationFilter filter);
    Task<TicketDto?> GetTicketById(string ticketId);
    Task<TicketDto?> UpdateTicketInfo(string ticketId, UdpateTicketDto updateTicketDto);
    Task<IEnumerable<TicketDto>> GetTicketsByPaymentId(string paymentId);
    Task<IEnumerable<TicketDto>> CreateTickets(string paymentId, CreateTicketsDto createTicketsDto);
    Task<IEnumerable<TicketDto>> ValidateTickets(string paymentId, bool isSuccess);
    Task<TicketDto?> TerminateTicket(string ticketId);
}