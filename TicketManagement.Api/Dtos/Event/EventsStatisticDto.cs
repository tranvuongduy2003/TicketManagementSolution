namespace TicketManagement.Api.Dtos;

public class EventsStatisticDto
{
    public int TotalEvents { get; set; } = 0;
    public int TotalBoughtTickets { get; set; } = 0;
    public decimal Revenue { get; set; } = 0;
}