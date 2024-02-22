using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class HighlightEventDto
{
    public EventDto? Event { get; set; } = null!;
    public HighlightType HighlightType { get; set; }
}