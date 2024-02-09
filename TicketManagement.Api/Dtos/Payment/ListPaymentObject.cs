namespace TicketManagement.Api.Dtos;

public class ListPaymentObject
{
    public IEnumerable<PaymentDto> payments { get; set; }
    public Metadata metadata { get; set; } 
}