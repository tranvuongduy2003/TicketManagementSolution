using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Contracts;

public interface IAlbumService
{
    Task<IEnumerable<AlbumDto>> GetAlbums();
    Task<IEnumerable<AlbumDto>> GetAlbumsByEventId(string eventId);
    Task<bool> AddImagesToAlbum(string eventId, IEnumerable<string> images);
    Task<bool> RemoveImagesFromAlbum(string eventId);
}