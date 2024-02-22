using System.Security.Claims;
using TicketManagement.Api.Models;

namespace TicketManagement.Api.Contracts;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    bool ValidateTokenExpire(string token);
}