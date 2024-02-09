namespace TicketManagement.Api.Dtos;

public class ListTicketObject
{
    public IEnumerable<TicketDto> Tickets { get; set; }
    public Metadata Metadata { get; set; }
}