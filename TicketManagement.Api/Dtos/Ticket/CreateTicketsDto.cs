namespace TicketManagement.Api.Dtos;

public class CreateTicketsDto
{
    public IEnumerable<TicketDto>? Tickets { get; set; }
}