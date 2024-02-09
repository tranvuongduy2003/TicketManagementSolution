using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Api.Dtos;

public class UpdateUserPasswordDto
{
    [Required]
    public string OldPassword { get; set; }
    [Required]
    public string NewPassword { get; set; }
}