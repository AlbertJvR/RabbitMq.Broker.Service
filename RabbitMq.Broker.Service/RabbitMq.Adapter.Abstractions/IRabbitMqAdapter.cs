using System;
using System.Threading.Tasks;

namespace RabbitMq.Adapter.Abstractions
{
    public interface IRabbitMqAdapter
    {
        Task CreateQueue(string queueName, bool durable = false, bool autoDelete = false);
        Task SubscribeToQueue(string queueName, Action<string> onMessageReceived);
        Task PublishMessageMessage(string queueName, string message, string routingKey = null);
    }
}
