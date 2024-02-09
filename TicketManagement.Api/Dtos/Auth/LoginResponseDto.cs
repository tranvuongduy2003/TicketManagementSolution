using TicketManagement.Api.Dtos;

namespace TicketManagement.Api.Dtos;

public class LoginResponseDto
{
    public UserDto User { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}