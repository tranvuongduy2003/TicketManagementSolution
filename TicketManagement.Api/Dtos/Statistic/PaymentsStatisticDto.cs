namespace TicketManagement.Api.Dtos.Statistic;

public class PaymentsStatisticDto
{
    public int TotalPayments { get; set; } = 0;
    public int TotalBoughtTickets { get; set; } = 0;
    public decimal TotalRevenue { get; set; } = 0;
}