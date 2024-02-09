using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public DateTime? DOB { get; set; } = null;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserGender? Gender { get; set; }
    public string? Avatar { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public string? RefreshToken { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}