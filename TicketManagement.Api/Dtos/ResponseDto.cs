namespace TicketManagement.Api.Dtos;

public class ResponseDto
{
    public object? Data { get; set; }
    public bool IsSuccess { get; set; } = true;
    public Metadata Meta { get; set; } = null;
    public string Message { get; set; } = "";
}