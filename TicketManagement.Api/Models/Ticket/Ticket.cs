using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Models;

public class Ticket
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string EventId { get; set; }
    public string? TicketCode { get; set; }
    public string OwnerName { get; set; }
    public string OwnerEmail { get; set; }
    public string OwnerPhone { get; set; }
    public string PaymentId { get; set; }
    public bool IsPaid { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [ForeignKey("EventId")] 
    public virtual Event Event { get; set; }
    [ForeignKey("PaymentId")] 
    public virtual Payment Payment { get; set; }
}