using TicketManagement.Api.Models;

namespace TicketManagement.Api.Contracts;

public interface IStatisticService
{
    Task<IEnumerable<Revenue>> GetGeneralStatistic();
}