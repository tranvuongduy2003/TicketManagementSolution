namespace TicketManagement.Api.Extensions;

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff);
    }
    
    public static DateTime StartOfMonth(this DateTime dt)
    {
        int month = dt.Month;
        int year = dt.Year;

        return new DateTime(year,month,1);
    }
    
    public static DateTime StartOfYear(this DateTime dt)
    {
        int year = dt.Year;

        return new DateTime(year,1,1);
    }
}
