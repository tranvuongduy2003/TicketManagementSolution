using AutoMapper;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Services;

public class AlbumService : IAlbumService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public AlbumService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<AlbumDto>> GetAlbums()
    {
        try
        {
            var albums = _db.Albums.ToList();
            var albumDtos = _mapper.Map<IEnumerable<AlbumDto>>(albums);

            return albumDtos;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<AlbumDto>> GetAlbumsByEventId(string eventId)
    {
        try
        {
            var albums = _db.Albums.Where(a => a.EventId == eventId).ToList();
            var albumDtos = _mapper.Map<IEnumerable<AlbumDto>>(albums);

            return albumDtos;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> AddImagesToAlbum(string eventId, IEnumerable<string> images)
    {
        try
        {
            await this.RemoveImagesFromAlbum(eventId);

            var albums = new List<Album>();
            
            await _db.Albums.AddRangeAsync(images.Select(i => new Album
            {
                EventId = eventId,
                Uri = i
            }));

            await _db.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> RemoveImagesFromAlbum(string eventId)
    {
        try
        {
            var albums = _db.Albums.Where(a => a.EventId == eventId).ToList();
            
            _db.Albums.RemoveRange(albums);

            await _db.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}