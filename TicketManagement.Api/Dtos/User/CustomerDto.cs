namespace TicketManagement.Api.Dtos;

public class CustomerDto : UserDto
{
    public int TotalBoughtTickets { get; set; } = 0;
}