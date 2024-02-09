namespace TicketManagement.Api.Dtos;

public class ListCategoryObject
{
    public IEnumerable<CategoryDto> categories { get; set; }
    public Metadata metadata { get; set; }
}