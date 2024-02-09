using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class UpdateUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DOB { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserGender? Gender { get; set; }
    public string? Avatar { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }
}