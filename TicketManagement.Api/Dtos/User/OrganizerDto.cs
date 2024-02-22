namespace TicketManagement.Api.Dtos;

public class OrganizerDto : UserDto
{
    public int? TotalEvents { get; set; } = 0;
    public int? TotalSoldTickets { get; set; } = 0;
}