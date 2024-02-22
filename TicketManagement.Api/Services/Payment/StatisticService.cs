using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Dtos.Statistic;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Services;

public class StatisticService : IStatisticService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public StatisticService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<GeneralStatisticDto> GetGeneralStatistic()
    {
        try
        {
            var totalEvents = _db.Events.Count();
            var totalBoughtTickets = _db.Tickets.Count();
            var totalCategories = _db.Categories.Count();
            var totalUsers = _db.Users.Count();

            return new GeneralStatisticDto
            {
                TotalEvents = totalEvents,
                TotalBoughtTickets = totalBoughtTickets,
                TotalCategories = totalCategories,
                TotalUsers = totalUsers,
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<Revenue>> GetRevenueStatistic()
    {
        try
        {
            var monthList = new List<Payment>()
            {
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 1, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 2, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 3, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 4, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 5, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 6, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 7, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 8, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 9, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 10, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 11, 1), TotalPrice = 0 },
                new Payment { CreatedAt = new DateTime(DateTime.Now.Year, 12, 1), TotalPrice = 0 },
            };

            var statistic = (from m in monthList
                    join p in _db.Payments.Where(p => p.CreatedAt.Year == DateTime.Now.Year) on m.CreatedAt.Month equals p.CreatedAt.Month into joinedPayments
                    from pj in joinedPayments.DefaultIfEmpty()
                    select new
                    {
                        CreatedAt = m.CreatedAt,
                        TotalPrice = pj != null ? pj.TotalPrice : 0
                    })
                .GroupBy(p => p.CreatedAt.Month)
                .Select(pl => new Revenue
                {
                    Month = pl.Key,
                    Value = pl.Sum(p => p.TotalPrice),
                });

            return statistic;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<EventStatisticDto>> GetEventsStatisticByCategory()
    {
        try
        {
            var statistic = _db.Events.Join(_db.Categories, e => e.CategoryId, c => c.Id, (e, c) =>
                    new Event
                    {
                        Id = e.Id,
                        CategoryId = e.CategoryId,
                        Category = c,
                    })
                .GroupBy(e => e.Category)
                .Select(pl => new EventStatisticDto()
                {
                    CategoryId = pl.Key.Id,
                    Category = pl.Key,
                    EventQuantity = pl.Count(),
                }).ToList();

            return statistic;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<EventsStatisticDto> GetEventsStatistic()
    {
        try
        {
            var totalEvents = _db.Events.Count();
            var totalBoughtTickets = _db.Tickets.Count();
            var revenue = _db.Payments.Sum(p => p.TotalPrice);

            return new EventsStatisticDto
            {
                TotalEvents = totalEvents,
                TotalBoughtTickets = totalBoughtTickets,
                Revenue = revenue
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<EventsStatisticDto> GetEventsStatisticByOrganizerId(string organizerId)
    {
        try
        {
            var events = _db.Events.Where(e => e.CreatorId == organizerId);
            var payments = _db.Payments.Join(events, p => p.EventId, e => e.Id, (p, e) => p);

            var totalEvents = events.Count();
            var totalBoughtTickets = payments.Sum(p => p.Quantity);
            var revenue = payments.Sum(p => p.TotalPrice);

            return new EventsStatisticDto
            {
                TotalEvents = totalEvents,
                TotalBoughtTickets = totalBoughtTickets,
                Revenue = revenue
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<PaymentsStatisticDto> GetPaymentsStatisticByEventId(string eventId)
    {
        try
        {
            var payments = _db.Payments.Where(p => p.EventId == eventId);
            
            var totalPayments = payments.Count();
            var totalBoughtTickets = payments.Sum(p => p.Quantity);
            var totalRevenue = payments.Sum(p => p.TotalPrice);
            
            return new PaymentsStatisticDto
            {
                TotalPayments = totalPayments,
                TotalBoughtTickets = totalBoughtTickets,
                TotalRevenue = totalRevenue
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}