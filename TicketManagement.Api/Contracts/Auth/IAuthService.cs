using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Contracts;

public interface IAuthService
{
    Task<string> Register(RegistrationRequestDto registrationRequestDto);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<bool> AssignRole(AssignRoleRequestDto model);
    Task<string> RefreshToken(string accessToken, string refreshToken);
    Task<UserDto?> GetUserProfile(string accessToken);
}
