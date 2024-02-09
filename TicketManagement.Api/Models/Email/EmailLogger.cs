using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketManagement.Api.Models;

public class EmailLogger
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public DateTime? EmailSent { get; set; }
}