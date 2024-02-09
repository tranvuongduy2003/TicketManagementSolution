using System.ComponentModel.DataAnnotations.Schema;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Models;

public class EventStatistic
{
    public string CategoryId { get; set; }
    public int EventQuantity { get; set; }
    
    [ForeignKey("CategoryId")] 
    public virtual Category Category { get; set; }  = null!;
}