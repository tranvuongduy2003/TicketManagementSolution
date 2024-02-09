namespace TicketManagement.Api.Dtos;

public class StripeRequestDto
{
    public string? StripeSessionUrl { get; set; }
    public string? StripeSessionId { get; set; }
    public string ApprovedUrl { get; set; }
    public string CancelUrl { get; set; }
    public string PaymentId { get; set; }
    public IEnumerable<TicketDto>? Tickets { get; set; }
}