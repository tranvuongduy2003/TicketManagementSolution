namespace TicketManagement.Api.Dtos;

public class GeneralStatisticDto
{
    public int TotalEvents { get; set; } = 0;
    public int TotalBoughtTickets { get; set; } = 0;
    public int TotalCategories { get; set; } = 0;
    public int TotalUsers { get; set; } = 0;
}