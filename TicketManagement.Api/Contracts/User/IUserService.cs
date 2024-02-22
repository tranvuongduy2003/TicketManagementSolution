using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Contracts;

public interface IUserService
{
    Task<ListUserObject<UserDto>> GetUsers(PaginationFilter filter);
    Task<ListUserObject<CustomerDto>> GetUsersInCustomer(PaginationFilter filter);
    Task<ListUserObject<OrganizerDto>> GetUsersInOrganizer(PaginationFilter filter);
    Task<UserDto?> GetUserById(string userId);
    Task<UserDto?> UpdateUser(string userId, UpdateUserDto updateUserDto);
    Task<string?> UpdateUserPassword(string userId, UpdateUserPasswordDto updateUserPasswordDto);
    Task<string?> UpdateUserStatus(string userId, UpdateUserStatusDto updateUserStatusDto);
}