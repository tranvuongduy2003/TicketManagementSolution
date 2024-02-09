namespace TicketManagement.Api.Dtos;

public class CheckoutDto
{
    public int EventId { get; set; }
    public string UserId { get; set; }
    public IEnumerable<TicketDto>? Tickets { get; set; }
    public double Discount { get; set; }
    public ContactInfoDto ContactInfo { get; set; }
    public int Quantity { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? StripeSessionId { get; set; }
}