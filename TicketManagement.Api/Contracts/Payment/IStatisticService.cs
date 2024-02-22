using TicketManagement.Api.Dtos;
using TicketManagement.Api.Dtos.Statistic;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Contracts;

public interface IStatisticService
{
    Task<GeneralStatisticDto> GetGeneralStatistic();
    Task<IEnumerable<Revenue>> GetRevenueStatistic();
    Task<IEnumerable<EventStatisticDto>> GetEventsStatisticByCategory();
    Task<EventsStatisticDto> GetEventsStatistic();
    Task<EventsStatisticDto> GetEventsStatisticByOrganizerId(string organizerId);
    Task<PaymentsStatisticDto> GetPaymentsStatisticByEventId(string eventId);
    
}