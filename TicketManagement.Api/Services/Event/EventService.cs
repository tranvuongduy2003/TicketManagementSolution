using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Enums;
using TicketManagement.Api.Extensions;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IAlbumService _albumService;
    private readonly ITicketService _ticketService;

    public EventService(AppDbContext db, IMapper mapper,
        IAlbumService albumService, ITicketService ticketService)
    {
        _db = db;
        _mapper = mapper;
        _albumService = albumService;
        _ticketService = ticketService;
    }

    public async Task<ListEventObject> GetEvents(EventFilter filter)
    {
        try
        {
            var events = _db.Events.ToList();
            if (filter.search != null)
            {
                events = events.Where(e => e.Name.ToLower().Contains(filter.search.ToLower())).ToList();
            }

            if (filter.categoryKeys != null && filter.categoryKeys.Count() > 0)
            {
                events = events.Where(e => filter.categoryKeys.Any(c => c == e.CategoryId)).ToList();
            }

            if (filter.price != null && filter.price.Count() > 0)
            {
                events = events.Where(e =>
                {
                    var valid = false;
                    foreach (var priceType in filter.price)
                    {
                        if (priceType == PriceType.FREE && e.TicketPrice == 0)
                            valid = true;
                        else if (priceType == PriceType.PAID && e.TicketPrice > 0)
                            valid = true;
                    }

                    return valid;
                }).ToList();
            }

            if (filter.time != null && filter.time.Count() > 0)
            {
                events = events.Where(e =>
                {
                    var valid = false;
                    foreach (var timeType in filter.time)
                    {
                        if (timeType == EventStatus.OPENING && e.StartTime <= DateTime.UtcNow &&
                            e.EndTime >= DateTime.UtcNow)
                            valid = true;
                        else if (timeType == EventStatus.UPCOMING && DateTime.UtcNow < e.StartTime)
                            valid = true;
                        else if (timeType == EventStatus.CLOSED && e.EndTime < DateTime.UtcNow)
                            valid = true;
                    }

                    return valid;
                }).ToList();
            }

            events = filter.order switch
            {
                PageOrder.ASC => events.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => events.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => events
            };

            var metadata = new Metadata(events.Count(), filter.page, filter.size, filter.takeAll);

            if (filter.takeAll == false)
            {
                events = events.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).Join(_db.Categories, e => e.CategoryId, c => c.Id, (e, c) =>
                    {
                        e.Category = c;
                        return e;
                    }).ToList();
            }

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return new ListEventObject
            {
                events = eventDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<EventDto>> GetTop3NewestEvents()
    {
        try
        {
            var events = _db.Events.Where(e => e.StartTime <= DateTime.UtcNow).OrderByDescending(e => e.StartTime)
                .Take(3).ToList();

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return eventDtos;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<EventDto>> Get24hUpcomingEvents()
    {
        try
        {
            var events = _db.Events.Where(e =>
                e.StartTime >= DateTime.UtcNow && e.StartTime <= DateTime.UtcNow.AddDays(1)).Take(3).ToList();

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return eventDtos;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<HighlightEventDto> GetHighlightEvent()
    {
        try
        {
            var startOfWeek = DateTime.UtcNow.StartOfWeek();
            var eventsInWeek = _db.Events.Where(e => e.StartTime >= startOfWeek && e.StartTime <= DateTime.UtcNow)
                .ToList();
            if (eventsInWeek.Any())
            {
                var highlightEvent = GetHighlightEventByOptionalEvents(eventsInWeek);

                var eventDto = _mapper.Map<EventDto>(highlightEvent);

                return new HighlightEventDto
                {
                    Event = eventDto,
                    HighlightType = HighlightType.WEEK,
                };
            }

            var startOfMonth = DateTime.Now.StartOfMonth();
            var eventsInMonth = _db.Events.Where(e =>
                e.StartTime >= startOfMonth && e.StartTime <= DateTime.UtcNow).ToList();
            if (eventsInMonth.Any())
            {
                var highlightEvent = GetHighlightEventByOptionalEvents(eventsInMonth);

                var eventDto = _mapper.Map<EventDto>(highlightEvent);

                return new HighlightEventDto
                {
                    Event = eventDto,
                    HighlightType = HighlightType.MONTH,
                };
            }

            var startOfYear = DateTime.Now.StartOfYear();
            var eventsInYear = _db.Events.Where(e =>
                e.StartTime >= startOfYear && e.StartTime <= DateTime.UtcNow).ToList();
            if (eventsInMonth.Any())
            {
                var highlightEvent = GetHighlightEventByOptionalEvents(eventsInYear);

                var eventDto = _mapper.Map<EventDto>(highlightEvent);

                return new HighlightEventDto
                {
                    Event = eventDto,
                    HighlightType = HighlightType.YEAR,
                };
            }

            return new HighlightEventDto
            {
                Event = null,
                HighlightType = HighlightType.NONE
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<EventDto>> GetHighlightEventsList()
    {
        try
        {
            var events = _db.Events.ToList();
            if (_db.Payments.Any())
            {
                var eventsByQuantity = _db.Payments.GroupBy(p => p.EventId).Select(groupedPayments => new
                {
                    EventId = groupedPayments.Key,
                    Quantity = groupedPayments.Sum(jp => jp.Quantity)
                });
                events = events.Join(eventsByQuantity, eiw => eiw.Id, ebq => ebq.EventId, (eiw, ebq) => new
                    {
                        Event = eiw,
                        Quantity = ebq.Quantity,
                    }).OrderByDescending(joinedEvents => joinedEvents.Quantity).Take(3)
                    .Select(joinedEvents => joinedEvents.Event).ToList();
            }
            else
            {
                events = events.Take(3).ToList();
            }

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return eventDtos;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<EventDto>> GetRandomEvents()
    {
        try
        {
            var random = new Random();

            var eventsLength = _db.Events.Count();

            var events = _db.Events.Skip(random.Next(eventsLength >= 3 ? eventsLength - 3 : 0)).Take(3).ToList();

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return eventDtos;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<ListEventObject> GetEventsByOrganizerId(string organizerId, PaginationFilter filter)
    {
        try
        {
            var events = _db.Events.Where(e => e.CreatorId == organizerId).ToList();
            if (filter.search != null)
            {
                events = events.Where(e => EF.Functions.Contains(e.Name, filter.search)).ToList();
            }

            events = filter.order switch
            {
                PageOrder.ASC => events.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => events.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => events
            };

            var metadata = new Metadata(events.Count(), filter.page, filter.size, filter.takeAll);

            if (filter.takeAll == false)
            {
                events = events.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);


            return new ListEventObject
            {
                events = eventDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<EventDto?> GetEventById(string eventId)
    {
        try
        {
            var eventFromDb = _db.Events.FirstOrDefault(u => u.Id == eventId);

            if (eventFromDb is null)
            {
                return null;
            }

            var eventDto = _mapper.Map<EventDto>(eventFromDb);

            var albums = await _albumService.GetAlbumsByEventId(eventDto.Id);

            eventDto.Album = albums.Select(a => a.Uri);

            return eventDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> CreateEvent(CreateEventDto createEventDto)
    {
        try
        {
            var newEvent = _mapper.Map<Event>(createEventDto);

            // if (newEvent.EventDate > DateTime.Now)
            // {
            //     newEvent.Status = EventStatus.UPCOMING;
            // }
            // else if (newEvent.EventDate.Year == DateTime.Now.Year && newEvent.EventDate.Month == DateTime.Now.Month &&
            //          newEvent.EventDate.Day == DateTime.Now.Month)
            // {
            //     newEvent.Status = EventStatus.OPENING;
            // }
            // else
            // {
            //     newEvent.Status = EventStatus.CLOSED;
            // }

            newEvent.Favourite = 0;
            newEvent.Share = 0;
            newEvent.TicketSoldQuantity = 0;

            newEvent.CreatedAt = DateTime.Now;
            newEvent.UpdatedAt = DateTime.Now;

            var createdEvent = await _db.Events.AddAsync(newEvent);


            await _db.SaveChangesAsync();

            await _albumService.AddImagesToAlbum(createdEvent.Entity.Id, createEventDto.Album);

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> UpdateEvent(string eventId, UpdateEventDto updateEventDto)
    {
        try
        {
            var eventFromDb = _db.Events.FirstOrDefault(u => u.Id == eventId);

            if (eventFromDb is null)
            {
                return false;
            }

            eventFromDb.CoverImage = updateEventDto.CoverImage;
            eventFromDb.Name = updateEventDto.Name;
            eventFromDb.Description = updateEventDto.Description;
            eventFromDb.CategoryId = updateEventDto.CategoryId;
            eventFromDb.Location = updateEventDto.Location;
            // eventFromDb.EventDate = updateEventDto.EventDate;
            eventFromDb.StartTime = updateEventDto.StartTime;
            eventFromDb.EndTime = updateEventDto.EndTime;
            eventFromDb.IsPromotion = updateEventDto.IsPromotion;
            eventFromDb.PromotionPlan = updateEventDto.PromotionPlan;
            eventFromDb.TicketPrice = updateEventDto.TicketPrice ?? 0;
            eventFromDb.TicketQuantity = updateEventDto.TicketQuantity;
            eventFromDb.TicketStartTime = updateEventDto.TicketStartTime;
            eventFromDb.TicketCloseTime = updateEventDto.TicketCloseTime;

            // if (eventFromDb.EventDate > DateTime.Now)
            // {
            //     eventFromDb.Status = EventStatus.UPCOMING;
            // }
            // else if (eventFromDb.EventDate.Year == DateTime.Now.Year &&
            //          eventFromDb.EventDate.Month == DateTime.Now.Month &&
            //          eventFromDb.EventDate.Day == DateTime.Now.Month)
            // {
            //     eventFromDb.Status = EventStatus.OPENING;
            // }
            // else
            // {
            //     eventFromDb.Status = EventStatus.CLOSED;
            // }

            eventFromDb.UpdatedAt = DateTime.Now;

            await _albumService.RemoveImagesFromAlbum(eventFromDb.Id);

            await _albumService.AddImagesToAlbum(eventId, updateEventDto.Album);

            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> DeleteEvent(string eventId)
    {
        try
        {
            var eventFromDb = _db.Events.FirstOrDefault(u => u.Id == eventId);

            if (eventFromDb is null)
            {
                return false;
            }

            _db.Events.Remove(eventFromDb);
            _db.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> IncreaseFavourite(string id)
    {
        try
        {
            var eventFromDb = _db.Events.FirstOrDefault(u => u.Id == id);

            if (eventFromDb is null)
            {
                return false;
            }

            eventFromDb.Favourite++;
            _db.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> DecreaseFavourite(string id)
    {
        try
        {
            var eventFromDb = _db.Events.FirstOrDefault(u => u.Id == id);

            if (eventFromDb is null)
            {
                return false;
            }

            if (eventFromDb.Favourite > 0)
            {
                eventFromDb.Favourite--;
            }

            _db.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> IncreaseShare(string id)
    {
        try
        {
            var eventFromDb = _db.Events.FirstOrDefault(u => u.Id == id);

            if (eventFromDb is null)
            {
                return false;
            }

            eventFromDb.Share++;
            _db.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> DecreaseShare(string id)
    {
        try
        {
            var eventFromDb = _db.Events.FirstOrDefault(u => u.Id == id);

            if (eventFromDb is null)
            {
                return false;
            }

            if (eventFromDb.Share > 0)
            {
                eventFromDb.Share--;
            }

            _db.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private Event? GetHighlightEventByOptionalEvents(List<Event> events)
    {
        if (_db.Payments.Any())
        {
            var eventsByQuantity = _db.Payments.GroupBy(p => p.EventId).Select(groupedPayments => new
            {
                EventId = groupedPayments.Key,
                Quantity = groupedPayments.Sum(jp => jp.Quantity)
            });
            var highlightEvent = events.Join(eventsByQuantity, eiw => eiw.Id, ebq => ebq.EventId, (eiw, ebq) => new
            {
                Event = eiw,
                Quantity = ebq.Quantity,
            }).MaxBy(joinedEvents => joinedEvents.Quantity);

            return highlightEvent != null ? highlightEvent.Event : null;
        }
        else
        {
            return events.FirstOrDefault();
        }
    }
}