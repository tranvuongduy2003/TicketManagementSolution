namespace TicketManagement.Api.Dtos;

public class CreateEventDto
{
    public string CoverImage { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public IEnumerable<string> Album { get; set; }
    public string Location { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TicketQuantity { get; set; }
    public decimal? TicketPrice { get; set; } = 0;
    public DateTime TicketStartTime { get; set; }
    public DateTime TicketCloseTime { get; set; }
    public Boolean IsPromotion { get; set; }
    public int? PromotionPlan { get; set; } = 0;
    public string CreatorId { get; set; }
}