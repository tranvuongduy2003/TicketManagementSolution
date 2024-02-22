using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Extensions;
using TicketManagement.Api.Contracts;
using TicketManagement.Api.Data;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Enums;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public AuthService(AppDbContext db, ITokenService tokenService,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _db = db;
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
    {
        var useWithEmailExisted = _db.ApplicationUsers.FirstOrDefault(u =>
            u.Email.ToLower() == registrationRequestDto.Email.ToLower());

        if (useWithEmailExisted != null) return "Email đã tồn tài";

        var useWithPhoneNumberExisted = _db.ApplicationUsers.FirstOrDefault(u =>
            u.PhoneNumber == registrationRequestDto.PhoneNumber);

        if (useWithPhoneNumberExisted != null) return "Số điện thoại đã tồn tài";

        ApplicationUser user = new()
        {
            UserName = registrationRequestDto.Email,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            Name = registrationRequestDto.Name,
            PhoneNumber = registrationRequestDto.PhoneNumber,
        };

        try
        {
            var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
            if (result.Succeeded)
            {
                var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                if (!_roleManager.RoleExistsAsync(registrationRequestDto.Role.GetDisplayName()).GetAwaiter()
                        .GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(registrationRequestDto.Role.GetDisplayName()))
                        .GetAwaiter().GetResult();
                }

                await _userManager.AddToRoleAsync(user, registrationRequestDto.Role.GetDisplayName());

                UserDto userDto = new()
                {
                    Email = userToReturn.Email,
                    Id = userToReturn.Id,
                    Name = userToReturn.Name,
                    PhoneNumber = userToReturn.PhoneNumber
                };
                
                user.UpdatedAt = DateTime.Now;
                user.CreatedAt = DateTime.Now;
                _db.SaveChanges();

                return "";
            }
            else
            {
                return result.Errors.FirstOrDefault().Description;
            }
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
        }
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        try
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u =>
                u.Email.ToLower() == loginRequestDto.Email.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user != null && user.Status == UserStatus.DEACTIVE)
            {
                throw new Exception("Tài khoản đã bị vô hiệu hóa");
            }

            if (user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, RefreshToken = null, AccessToken = null };
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.UpdatedAt = DateTime.Now;
            _db.SaveChanges();

            var userRoles = await _userManager.GetRolesAsync(user);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = Enum.Parse<UserRole>(userRoles.FirstOrDefault());

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return loginResponseDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> AssignRole(AssignRoleRequestDto assignRoleRequestDto)
    {
        try
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u =>
                u.Email.ToLower() == assignRoleRequestDto.Email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(assignRoleRequestDto.Role.GetDisplayName()).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(assignRoleRequestDto.Role.GetDisplayName())).GetAwaiter()
                        .GetResult();
                }

                await _userManager.AddToRoleAsync(user, assignRoleRequestDto.Role.GetDisplayName());
                
                user.UpdatedAt = DateTime.Now;
                _db.SaveChanges();
                
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<string> RefreshToken(string accessToken, string refreshToken)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null) return "";

            var userEmail = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == userEmail.ToLower());
            if (user is null || user.RefreshToken != refreshToken || !_tokenService.ValidateTokenExpire(refreshToken))
            {
                return "";
            }

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles);

            return newAccessToken;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<UserDto?> GetUserProfile(string accessToken)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null) return null;

            var userEmail = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            // var userProvider = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod).Value;

            var user = new ApplicationUser();
            
            user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == userEmail.ToLower());
            if (user is null) return null;

            var userRoles = await _userManager.GetRolesAsync(user);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = Enum.Parse<UserRole>(userRoles.FirstOrDefault());

            return userDto;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}