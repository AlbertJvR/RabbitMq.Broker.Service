using RabbitMq.Adapter.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Broker.Tests
{
    public class MockRabbitMqAdapter : IRabbitMqAdapter
    {
        private readonly MockRabbitMqQueue _queue;

        public MockRabbitMqAdapter(MockRabbitMqQueue queue)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public Task CreateQueue(string queueName, bool durable = false, bool autoDelete = false)
        {
            _queue.CreateQueue(queueName);
            return Task.CompletedTask;
        }

        public Task PublishMessageMessage(string queueName, string message, string routingKey = null)
        {
            _queue.PublishMessageMessage(queueName, Encoding.UTF8.GetBytes(message));
            return Task.CompletedTask;
        }

        public Task SubscribeToQueue(string queueName, Action<string> onMessageReceived)
        {
            _queue.SubscribeToQueue(queueName,(a) => onMessageReceived.Invoke(Encoding.UTF8.GetString(a)));
            return Task.CompletedTask;
        }
    }
}
