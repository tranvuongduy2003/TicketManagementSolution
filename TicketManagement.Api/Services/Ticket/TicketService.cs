using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Enums;
using TicketManagement.Api.Models;
using TicketManagement.Api.Utility;

namespace TicketManagement.Api.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public TicketService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ListTicketObject> GetTickets(PaginationFilter filter)
    {
        try
        {
            var tickets = _db.Tickets.ToList();
            if (filter.search != null)
            {
                var searchValue = filter.search.ToLower();
                tickets = tickets.Where(t =>
                        EF.Functions.Contains(t.OwnerName, searchValue) ||
                        EF.Functions.Contains(t.OwnerEmail, searchValue) ||
                        EF.Functions.Contains(t.TicketCode ?? "", searchValue))
                    .ToList();
            }

            tickets = filter.order switch
            {
                PageOrder.ASC => tickets.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => tickets.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => tickets
            };

            var metadata = new Metadata(tickets.Count(), filter.page, filter.size);

            if (filter.takeAll == false)
            {
                tickets = tickets.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var ticketDtos = tickets.Join(_db.Events, t => t.EventId, e => e.Id, (t, e) => new TicketDto
            {
                Id = t.Id,
                OwnerName = t.OwnerName,
                OwnerEmail = t.OwnerEmail,
                OwnerPhone = t.OwnerPhone,
                StartTime = e.StartTime,
                CloseTime = e.TicketCloseTime,
                Price = e.TicketPrice,
                Status = t.Status,
                EventId = e.Id,
                TicketCode = t.TicketCode
            }).ToList();

            return new ListTicketObject
            {
                Tickets = ticketDtos,
                Metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<ListTicketObject> GetTicketsByUserId(string userId, PaginationFilter filter)
    {
        try
        {
            var tickets = _db.Tickets.Join(_db.Payments.Where(p => p.UserId == userId), t => t.PaymentId, p => p.Id,
                (t, p) => t).ToList();
            if (filter.search != null)
            {
                var searchValue = filter.search.ToLower();
                tickets = tickets.Where(t =>
                        EF.Functions.Contains(t.OwnerName, searchValue) ||
                        EF.Functions.Contains(t.OwnerEmail, searchValue) ||
                        EF.Functions.Contains(t.TicketCode ?? "", searchValue))
                    .ToList();
            }

            tickets = filter.order switch
            {
                PageOrder.ASC => tickets.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => tickets.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => tickets
            };

            var metadata = new Metadata(tickets.Count(), filter.page, filter.size);

            if (filter.takeAll == false)
            {
                tickets = tickets.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var ticketDtos = tickets.Join(_db.Events, t => t.EventId, e => e.Id, (t, e) => new TicketDto
            {
                Id = t.Id,
                OwnerName = t.OwnerName,
                OwnerEmail = t.OwnerEmail,
                OwnerPhone = t.OwnerPhone,
                StartTime = e.TicketStartTime,
                CloseTime = e.TicketCloseTime,
                Status = t.Status,
                Price = e.TicketPrice,
                EventId = e.Id,
                TicketCode = t.TicketCode
            }).ToList();

            return new ListTicketObject
            {
                Tickets = ticketDtos,
                Metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<TicketDto?> GetTicketById(string ticketId)
    {
        try
        {
            var ticket = (from t in _db.Tickets
                join e in _db.Events on t.EventId equals e.Id
                select new TicketDto
                {
                    Id = t.Id,
                    OwnerName = t.OwnerName,
                    OwnerEmail = t.OwnerEmail,
                    OwnerPhone = t.OwnerPhone,
                    StartTime = e.TicketStartTime,
                    CloseTime = e.TicketCloseTime,
                    Status = t.Status,
                    Price = e.TicketPrice,
                    EventId = e.Id,
                    TicketCode = t.TicketCode
                }).FirstOrDefault(t => t.Id == ticketId);

            if (ticket is null)
            {
                return null;
            }

            // var ticketDto = _mapper.Map<TicketDto>(ticket);

            return ticket;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<TicketDto?> UpdateTicketInfo(string ticketId, UdpateTicketDto updateTicketDto)
    {
        try
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return null;
            }

            ticket.OwnerName = updateTicketDto.Fullname;
            ticket.OwnerEmail = updateTicketDto.Email;
            ticket.OwnerPhone = updateTicketDto.Phone;

            ticket.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return _mapper.Map<TicketDto>(ticket);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<TicketDto>> GetTicketsByPaymentId(string paymentId)
    {
        try
        {
            var tickets = (from t in _db.Tickets
                join e in _db.Events on t.EventId equals e.Id
                where t.PaymentId == paymentId
                select new TicketDto
                {
                    Id = t.Id,
                    OwnerName = t.OwnerName,
                    OwnerEmail = t.OwnerEmail,
                    OwnerPhone = t.OwnerPhone,
                    StartTime = e.TicketStartTime,
                    CloseTime = e.TicketCloseTime,
                    Status = t.Status,
                    Price = e.TicketPrice,
                    EventId = e.Id,
                    TicketCode = t.TicketCode
                }).ToList();

            return tickets;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<TicketDto>> CreateTickets(string paymentId, CreateTicketsDto createTicketsDto)
    {
        try
        {
            var tickets = _mapper.Map<IEnumerable<Ticket>>(createTicketsDto.Tickets);

            foreach (var ticket in tickets)
            {
                var ticketEvent =
                    _db.Events.First(e => e.Id == createTicketsDto.Tickets.First().EventId);

                ticket.PaymentId = paymentId;
                ticket.EventId = ticketEvent.Id;
                ticket.IsPaid = false;
                ticket.Status = TicketStatus.PENDING;
                ticket.CreatedAt = DateTime.Now;
                ticket.UpdatedAt = DateTime.Now;
            }

            _db.Tickets.AddRange(tickets);
            _db.SaveChanges();

            return _mapper.Map<IEnumerable<TicketDto>>(tickets);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<TicketDto>> ValidateTickets(string paymentId, bool isSuccess)
    {
        try
        {
            var tickets = _db.Tickets.Where(t => t.PaymentId == paymentId).ToList();

            if (isSuccess)
            {
                foreach (var ticket in tickets)
                {
                    ticket.TicketCode =
                        TicketCodeGenerator.HashSHA512String(ticket.OwnerName, ticket.OwnerEmail, ticket.OwnerPhone);
                    ticket.IsPaid = true;
                    if (ticket.Status != TicketStatus.TERMINATED)
                    {
                        ticket.Status = TicketStatus.PAID;
                    }

                    ticket.UpdatedAt = DateTime.Now;
                }

                var ticketEvent = _db.Events.First(td => td.Id == tickets.First().EventId);
                ticketEvent.TicketSoldQuantity += tickets.Count();

                _db.SaveChanges();

                var ticketDtos = (from t in tickets
                    join e in _db.Events on t.EventId equals e.Id
                    where t.PaymentId == paymentId
                    select new TicketDto
                    {
                        Id = t.Id,
                        OwnerName = t.OwnerName,
                        OwnerEmail = t.OwnerEmail,
                        OwnerPhone = t.OwnerPhone,
                        StartTime = e.TicketStartTime,
                        CloseTime = e.TicketCloseTime,
                        Price = e.TicketPrice,
                        EventId = e.Id,
                        TicketCode = t.TicketCode
                    }).ToList();

                return ticketDtos;
            }
            else
            {
                _db.Tickets.RemoveRange(tickets);
                _db.SaveChanges();
                return null;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<TicketDto?> TerminateTicket(string ticketId)
    {
        try
        {
            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return null;
            }

            ticket.Status = TicketStatus.TERMINATED;

            await _db.SaveChangesAsync();

            return _mapper.Map<TicketDto>(ticket);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}