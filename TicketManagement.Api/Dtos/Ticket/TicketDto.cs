using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class TicketDto
{
    public string Id { get; set; }
    public string? TicketCode { get; set; }
    public string OwnerName { get; set; }
    public string OwnerEmail { get; set; }
    public string OwnerPhone { get; set; }
    public string EventId { get; set; }
    public bool IsPaid { get; set; }
    public decimal Price { get; set; }
    public TicketStatus? Status { get; set; } = TicketStatus.PENDING;
    public DateTime StartTime { get; set; }
    public DateTime CloseTime { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}