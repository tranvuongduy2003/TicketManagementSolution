using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Enums;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IMapper mapper)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<ListUserObject<UserDto>> GetUsers(PaginationFilter filter)
    {
        try
        {
            var users = _userManager.Users.ToList();
            if (filter.search != null)
            {
                users = users.Where(u => EF.Functions.Contains(u.Name, filter.search) || EF.Functions.Contains(u.Email, filter.search)).ToList();
            } 
            users = filter.order switch
            {
                PageOrder.ASC => users.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => users.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => users
            };

            var metadata = new Metadata(users.Count(), filter.page, filter.size, filter.takeAll);
            
            if (filter.takeAll == false)
            { 
                users = users.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var userDtos = users.Select(u =>
            {
                var userDto = _mapper.Map<UserDto>(u);
                var userRoles = _userManager.GetRolesAsync(u).GetAwaiter().GetResult();
                var userRole = Enum.Parse<UserRole>(userRoles.FirstOrDefault());
                userDto.Role = userRole;
                return userDto;
            });

            return new ListUserObject<UserDto>
            {
                users = userDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
    public async Task<ListUserObject<CustomerDto>> GetUsersInCustomer(PaginationFilter filter)
    {
        try
        {
            var users = await _userManager.GetUsersInRoleAsync(UserRole.CUSTOMER.GetDisplayName());
            if (filter.search != null)
            {
                users = users.Where(u => EF.Functions.Contains(u.Name, filter.search) || EF.Functions.Contains(u.Email, filter.search)).ToList();
            } 
            users = filter.order switch
            {
                PageOrder.ASC => users.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => users.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => users
            };

            var metadata = new Metadata(users.Count(), filter.page, filter.size, filter.takeAll);
            
            if (filter.takeAll == false)
            { 
                users = users.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var userDtos = users.Select(u =>
            {
                var userDto = _mapper.Map<CustomerDto>(u);
                var userRoles = _userManager.GetRolesAsync(u).GetAwaiter().GetResult();
                var userRole = Enum.Parse<UserRole>(userRoles.FirstOrDefault());
                userDto.Role = userRole;

                userDto.TotalBoughtTickets = _db.Payments.Where(p => p.UserId == u.Id).Sum(p => p.Quantity);
                
                return userDto;
            });

            return new ListUserObject<CustomerDto>
            {
                users = userDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
    public async Task<ListUserObject<OrganizerDto>> GetUsersInOrganizer(PaginationFilter filter)
    {
        try
        {
            var users = await _userManager.GetUsersInRoleAsync(UserRole.ORGANIZER.GetDisplayName());
            if (filter.search != null)
            {
                users = users.Where(u => EF.Functions.Contains(u.Name, filter.search) || EF.Functions.Contains(u.Email, filter.search)).ToList();
            } 
            users = filter.order switch
            {
                PageOrder.ASC => users.OrderBy(c => c.CreatedAt).ToList(),
                PageOrder.DESC => users.OrderByDescending(c => c.CreatedAt).ToList(),
                _ => users
            };

            var metadata = new Metadata(users.Count(), filter.page, filter.size, filter.takeAll);
            
            if (filter.takeAll == false)
            { 
                users = users.Skip((filter.page - 1) * filter.size)
                    .Take(filter.size).ToList();
            }

            var userDtos = users.Select(u =>
            {
                var userDto = _mapper.Map<OrganizerDto>(u);
                var userRoles = _userManager.GetRolesAsync(u).GetAwaiter().GetResult();
                var userRole = Enum.Parse<UserRole>(userRoles.FirstOrDefault());
                userDto.Role = userRole;

                userDto.TotalEvents = _db.Events.Count(e => e.CreatorId == u.Id);
                userDto.TotalSoldTickets = _db.Payments.Join(_db.Events, p => p.EventId, e => e.Id, (p, e) => new
                {
                    Quantity = p.Quantity,
                    CreatorId = e.CreatorId
                }).Where(p => p.CreatorId == u.Id).Sum(p => p.Quantity);
                
                return userDto;
            });

            return new ListUserObject<OrganizerDto>
            {
                users = userDtos,
                metadata = metadata
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UserDto?> GetUserById(string userId)
    {
        try
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = Enum.Parse<UserRole>(userRoles.FirstOrDefault());
            switch (userDto.Role)
            {
                case UserRole.CUSTOMER:
                    var customerDto = _mapper.Map<CustomerDto>(userDto);
                    customerDto.TotalBoughtTickets = _db.Payments.Where(p => p.UserId == userDto.Id).Sum(p => p.Quantity);
                    return customerDto;
                case UserRole.ORGANIZER:
                    var organizerDto = _mapper.Map<OrganizerDto>(userDto);
                    organizerDto.TotalEvents = _db.Events.Count(e => e.CreatorId == userDto.Id);
                    organizerDto.TotalSoldTickets = _db.Payments.Join(_db.Events, p => p.EventId, e => e.Id, (p, e) => new
                    {
                        Quantity = p.Quantity,
                        CreatorId = e.CreatorId
                    }).Where(p => p.CreatorId == userDto.Id).Sum(p => p.Quantity);
                    return organizerDto;
            }

            return userDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UserDto?> UpdateUser(string userId, UpdateUserDto updateUserDto)
    {
        try
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if (user is null)
            {
                return null;
            }

            var updatedUser = _mapper.Map<ApplicationUser>(updateUserDto);
            user.Name = updateUserDto.Name;
            user.Avatar = updateUserDto.Avatar;
            user.Email = updateUserDto.Email;
            user.Gender = updateUserDto.Gender;
            user.Status = updateUserDto.Status;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.DOB = updateUserDto.DOB;
            user.UpdatedAt = DateTime.Now;
            _db.SaveChanges();

            if (!_roleManager.RoleExistsAsync(updateUserDto.Role.GetDisplayName()).GetAwaiter()
                    .GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(updateUserDto.Role.GetDisplayName()))
                    .GetAwaiter().GetResult();
            }

            var userRoles = await _userManager.GetRolesAsync(updatedUser);
            await _userManager.RemoveFromRolesAsync(updatedUser, userRoles);
            await _userManager.AddToRoleAsync(updatedUser, updateUserDto.Role.GetDisplayName());


            var updatedUserDto = _mapper.Map<UserDto>(updatedUser);
            return updatedUserDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<string?> UpdateUserPassword(string userId, UpdateUserPasswordDto updateUserPasswordDto)
    {
        try
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (user is null)
            {
                return null;
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, updateUserPasswordDto.OldPassword);
            if (!isValid)
            {
                return "Old password is wrong";
            }

            await _userManager.ChangePasswordAsync(user, updateUserPasswordDto.OldPassword,
                updateUserPasswordDto.NewPassword);

            user.UpdatedAt = DateTime.Now;
            _db.SaveChanges();

            return "";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
    public async Task<string?> UpdateUserStatus(string userId, UpdateUserStatusDto updateUserStatusDto)
    {
        try
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (user is null)
            {
                return null;
            }

            user.Status = updateUserStatusDto.Status;
            user.UpdatedAt = DateTime.Now;
            _db.SaveChanges();

            return "";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}