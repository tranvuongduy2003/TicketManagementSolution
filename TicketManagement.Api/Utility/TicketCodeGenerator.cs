namespace TicketManagement.Api.Utility;

public static class TicketCodeGenerator
{
    public static string HashSHA512String(string OwnerName, string OwnerEmail, string OwnerPhone)
    {
        string AllString = OwnerName + OwnerEmail + OwnerPhone + DateTime.UtcNow;
        if (string.IsNullOrEmpty(AllString)) throw new ArgumentNullException();
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(AllString);
        buffer = System.Security.Cryptography.SHA512.Create().ComputeHash(buffer);
        
        return Convert.ToHexString(buffer);
    }
}