using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class UserDto
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DOB { get; set; }
    public string? Gender { get; set; }
    public string? Avatar { get; set; }
    public int? TotalBuyedTickets { get; set; }
    public int? TotalEvents { get; set; }
    public int? TotalSoldTickets { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus UserStatus { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}