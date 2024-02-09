namespace TicketManagement.Api.Messaging.IMessaging;

public interface IAzureServiceBusConsumer
{
    Task Start();
    Task Stop();
}