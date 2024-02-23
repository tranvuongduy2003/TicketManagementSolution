using System.ComponentModel;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class EventFilter : PaginationFilter
{
    public IEnumerable<string>? categoryKeys { get; set; } = new List<string>();
    public IEnumerable<PriceType>? price { get; set; } =  new List<PriceType>();
    public IEnumerable<EventStatus>? time { get; set; } =  new List<EventStatus>();
}