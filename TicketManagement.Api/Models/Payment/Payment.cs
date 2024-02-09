using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Models;

public class Payment
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string EventId { get; set; }
    public string UserId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public double Discount { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerPhone { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus Status { get; set; }

    public string? PaymentMethod { get; set; } = "";
    public string? PaymentIntentId { get; set; }
    public string? StripeSessionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [ForeignKey("EventId")] 
    public virtual Event Event { get; set; }  = null!;
    [ForeignKey("UserId")] 
    public virtual ApplicationUser User { get; set; }  = null!;
}