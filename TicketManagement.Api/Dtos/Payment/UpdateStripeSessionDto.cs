namespace TicketManagement.Api.Dtos;

public class UpdateStripeSessionDto
{
    public IEnumerable<TicketDto> Tickets { get; set; }
    public string SessionId { get; set; }
}