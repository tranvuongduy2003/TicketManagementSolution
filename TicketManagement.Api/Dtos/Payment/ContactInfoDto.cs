using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Api.Dtos;

public class ContactInfoDto
{
    [Required]
    public string FullName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
}