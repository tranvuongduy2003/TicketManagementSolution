using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Enums;
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

    public async Task<ListEventObject> GetEvents(PaginationFilter filter)
    {
        try
        {
            var events = _db.Events.ToList();
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

            var metadata = new Metadata(events.Count(), filter.page, filter.size);
            
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

    public async Task<IEnumerable<EventStatistic>> GetEventsStatisticByCategory()
    {
        try
        {
            var statistic = await _db.Events
                .GroupBy(e => e.CategoryId)
                .Select(pl => new EventStatistic()
                {
                    CategoryId = pl.Key,
                    EventQuantity = pl.Count(),
                }).ToListAsync();

            return statistic;
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

            var metadata = new Metadata(events.Count(), filter.page, filter.size);
            
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
}