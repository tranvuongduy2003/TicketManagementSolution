using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagement.Api.Models;

public class Album
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string EventId { get; set; }
    public string Uri { get; set; }
    
    [ForeignKey("EventId")] 
    public virtual Event Event { get; set; }  = null!;
}