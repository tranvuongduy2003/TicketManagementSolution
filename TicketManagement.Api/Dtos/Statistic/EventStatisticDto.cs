using System.ComponentModel.DataAnnotations.Schema;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Models;

public class EventStatisticDto
{
    public string CategoryId { get; set; }
    public int EventQuantity { get; set; }
    public Category? Category { get; set; }  = null!;
}