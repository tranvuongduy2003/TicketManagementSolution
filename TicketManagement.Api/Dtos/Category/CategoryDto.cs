namespace TicketManagement.Api.Dtos;

public class CategoryDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int? TotalEvents { get; set; } = 0;
    public int? TotalTickets { get; set; } = 0;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}