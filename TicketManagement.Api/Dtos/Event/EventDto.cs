using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class EventDto
{
    public string Id { get; set; }
    public string CoverImage { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public string CreatorId { get; set; }
    public string Location { get; set; }
    public IEnumerable<string> Album { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsPromotion { get; set; }
    public int? PromotionPlan { get; set; }
    public decimal TicketPrice { get; set; }
    public int TicketSoldQuantity { get; set; }
    public int TicketQuantity { get; set; }
    public DateTime TicketStartTime { get; set; }
    public DateTime TicketCloseTime { get; set; }
    public EventStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}