namespace TicketManagement.Api.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}