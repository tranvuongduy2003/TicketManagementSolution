using TicketManagement.Api.Enums;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Dtos;

public class PaymentDto
{
    public string Id { get; set; }
    public string EventId { get; set; }
    public string UserId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public double Discount { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    public PaymentStatus Status { get; set; }
    public string PaymentMethod { get; set; }
    public IEnumerable<TicketDto>? Tickets { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? StripeSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Event? Event { get; set; }
    public ApplicationUser? User { get; set; }
}
