using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Broker.Tests
{
    public class MockRabbitMqQueue
    {
        private readonly ConcurrentBag<string> _queues = new ConcurrentBag<string>();
        private readonly ConcurrentDictionary<string,ConcurrentBag<Action<byte[]>>> _subscriptions = new ConcurrentDictionary<string, ConcurrentBag<Action<byte[]>>>();
        public void CreateQueue(string queueName)
        {
            _queues.Add(queueName);
        }

        public void PublishMessageMessage(string queueName, byte[] message, string routingKey = null)
        {
            if (_subscriptions.TryGetValue(queueName, out var subscribers))
                foreach (var subscriber in subscribers)
                    subscriber.Invoke(message);
        }

        public void SubscribeToQueue(string queueName, Action<byte[]> onMessageReceived)
        {
            if (!_queues.Contains(queueName))
                throw new InvalidOperationException($"{queueName} does not exist");

            _subscriptions.AddOrUpdate(queueName, new ConcurrentBag<Action<byte[]>>() {onMessageReceived},
                (key, oldValue) =>
                {
                    oldValue.Add(onMessageReceived);
                    return oldValue;
                });
        }
    }
}
