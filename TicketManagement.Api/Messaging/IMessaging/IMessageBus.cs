namespace TicketManagement.Api.Messaging.IMessaging;

public interface IMessageBus
{
    Task PublishMessage(object message, string topic_queue_Name);
}