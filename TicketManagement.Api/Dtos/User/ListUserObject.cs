namespace TicketManagement.Api.Dtos;

public class ListUserObject<T>
{
    public IEnumerable<T> users { get; set; }
    public Metadata metadata { get; set; } 
}