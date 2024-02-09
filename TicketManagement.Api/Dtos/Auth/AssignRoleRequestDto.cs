using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class AssignRoleRequestDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }
}