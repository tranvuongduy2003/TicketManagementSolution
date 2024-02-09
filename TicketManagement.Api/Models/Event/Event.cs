using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Models;

public class Event
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string CoverImage { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public string Location { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string CreatorId { get; set; }
    public bool IsPromotion { get; set; }
    public int? PromotionPlan { get; set; }
    public int Favourite { get; set; }
    public int Share { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public int TicketQuantity { get; set; }
    public int TicketSoldQuantity { get; set; }
    public decimal TicketPrice { get; set; }
    public DateTime TicketStartTime { get; set; }
    public DateTime TicketCloseTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [ForeignKey("CategoryId")] 
    public virtual Category Category { get; set; }  = null!;
    [ForeignKey("CreatorId")] 
    public virtual ApplicationUser Creator { get; set; }  = null!;
}