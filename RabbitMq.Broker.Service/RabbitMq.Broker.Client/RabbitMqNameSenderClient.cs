using Microservice.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMq.Adapter.Abstractions;
using RabbitMq.Broker.Client.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMq.Broker.Client
{
    public class RabbitMqNameSenderClient : IStartup
    {
        private readonly IRabbitMqAdapter _adapter;
        private ILogger<RabbitMqNameSenderClient> _logger;
        private RabbitMqClientOptions _options;

        public RabbitMqNameSenderClient(IRabbitMqAdapter adapter, ILogger<RabbitMqNameSenderClient> logger,IOptions<RabbitMqClientOptions> options)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<bool> SendName(string prefix,string name)
        {
            var semaphoreSlim = new SemaphoreSlim(1);
            await semaphoreSlim.WaitAsync();
            var success = new ThreadSafeSingleUpdateValue();
            await _adapter.CreateQueue(_options.SubscribeQueueName);
            Action<string> messageSub = message =>
            {
                _logger.LogInformation($"Message received: {message}");
                if (message == $"Hello {message.Substring(prefix.Length)}, I am your father!")
                {
                    success.UpdateValue(true);
                    semaphoreSlim.Release();
                }
                else if (message == "Error, name couldn't be determined")
                {
                    success.UpdateValue(false);
                    semaphoreSlim.Release();
                }

                success.UpdateValue(true);
                semaphoreSlim.Release();
            };
            await _adapter.SubscribeToQueue(_options.SubscribeQueueName, messageSub);
            await _adapter.PublishMessageMessage(_options.PublishQueueName, $"{prefix}{name}");
            if (!await semaphoreSlim.WaitAsync(_options.TimeoutMilliseconds))
                return false;
            return success.Successful ?? false;
        }

        public async Task Run()
        {
            var prefix = "Hello my name is, ";
            Console.Write("Please enter your name: ");
            var name = Console.ReadLine();
            await SendName(prefix, name);
            Console.ReadKey();
        }
    }
}
