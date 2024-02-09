using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class RegistrationRequestDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Password { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole? Role { get; set; }
}