using System.Text.Json.Serialization;
using TicketManagement.Api.Enums;

namespace TicketManagement.Api.Dtos;

public class UpdateUserStatusDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; set; }
}