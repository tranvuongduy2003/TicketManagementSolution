using TicketManagement.Api.Dtos;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Contracts;

public interface IEventService
{
    Task<ListEventObject> GetEvents(PaginationFilter filter);
    Task<IEnumerable<EventStatistic>> GetEventsStatisticByCategory();
    Task<ListEventObject> GetEventsByOrganizerId(string organizerId, PaginationFilter filter);
    Task<EventDto?> GetEventById(string eventId);
    Task<bool> CreateEvent(CreateEventDto createEventDto);
    Task<bool> UpdateEvent(string eventId, UpdateEventDto updateEventDto);
    Task<bool> DeleteEvent(string eventId);
    Task<bool> IncreaseFavourite(string id);
    Task<bool> DecreaseFavourite(string id);
    Task<bool> IncreaseShare(string id);
    Task<bool> DecreaseShare(string id);
}