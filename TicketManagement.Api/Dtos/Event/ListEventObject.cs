namespace TicketManagement.Api.Dtos;

public class ListEventObject
{
    public IEnumerable<EventDto> events { get; set; }
    public Metadata metadata { get; set; }
}