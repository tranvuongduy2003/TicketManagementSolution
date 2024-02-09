using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Api.Dtos;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; }
}