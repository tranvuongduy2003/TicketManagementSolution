namespace TicketManagement.Api.Dtos;

public class EventsByTicketsDto
{
    public string? Search { get; set; }
    public IEnumerable<int> EventIds { get; set; }
}