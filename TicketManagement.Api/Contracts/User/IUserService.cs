using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Contracts;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsers();
    Task<UserDto?> GetUserById(string userId);
    Task<UserDto?> UpdateUser(string userId, UpdateUserDto updateUserDto);
    Task<string?> UpdateUserPassword(string userId, UpdateUserPasswordDto updateUserPasswordDto);
    Task<string?> UpdateUserStatus(string userId, UpdateUserStatusDto updateUserStatusDto);
}