using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microservice.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMq.Adapter.Abstractions;
using RabbitMq.Broker.Service.Options;

namespace RabbitMq.Broker.Service
{
    public class RabbitMqNameListenerService:IStartup
    {
        private readonly IRabbitMqAdapter _adapter;
        private ILogger<RabbitMqNameListenerService> _logger;
        private RabbitMqServiceOptions _options;

        public RabbitMqNameListenerService(IRabbitMqAdapter adapter,ILogger<RabbitMqNameListenerService> logger,IOptions<RabbitMqServiceOptions> options)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task ListenToMessages()
        {
            var prefix = "Hello my name is, ";
            await _adapter.CreateQueue(_options.SubscribeQueueName);
            await _adapter.SubscribeToQueue(_options.SubscribeQueueName, async message =>
            {
                _logger.LogInformation($"Message received: {message}");
                if (message.IndexOf(prefix) == 0)
                {
                    await _adapter.PublishMessageMessage(_options.PublishQueueName, $"Hello {message.Substring(prefix.Length)}, I am your father!");
                }
                else
                {
                    _logger.LogWarning("Information not in correct format");
                    await _adapter.PublishMessageMessage(_options.PublishQueueName, $"Error, name couldn't be determined");
                }
            });
        }

        public async Task Run()
        {
            await ListenToMessages();
            Console.ReadKey();
        }
    }
}
