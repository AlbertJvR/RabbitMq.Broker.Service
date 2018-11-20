using RabbitMq.Adapter.Abstractions;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.Adapter.Client
{
    public class RabbitMqAdapter : IRabbitMqAdapter
    {
        private RabbitMqOptions _options;
        private ConnectionFactory _factory;
        private IModel _channel;
        private EventingBasicConsumer _consumer;
        private ILogger<RabbitMqAdapter> _logger;

        public RabbitMqAdapter(IOptions<RabbitMqOptions> options,ILogger<RabbitMqAdapter> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _factory = new ConnectionFactory() { HostName = _options.HostName };
        }

        public Task CreateQueue(string queueName,bool durable=false,bool autoDelete=false)
        {
            _logger.LogInformation($"Creating queue: {queueName} D:{durable} AD:{autoDelete}");
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                    durable: durable,
                    autoDelete: autoDelete,
                    exclusive: false,
                    arguments: null
                    );
            }
            return Task.CompletedTask;
        }

        public Task SubscribeToQueue(string queueName, Action<string> onMessageReceived)
        {
            _logger.LogInformation($"Subscribing to queue: {queueName}");
            _channel = _channel ?? _factory.CreateConnection().CreateModel();
            _consumer = _consumer??new EventingBasicConsumer(_channel);
            _consumer.Received += (model, args) => onMessageReceived(Encoding.UTF8.GetString(args.Body));
            _channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: _consumer);
            return Task.CompletedTask;
        }

        public Task PublishMessageMessage(string queueName,string message, string routingKey=null)
        {
            _logger.LogInformation($"Publishing message to queue: {queueName}");
            routingKey = routingKey ?? $"{queueName}";
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
            }

            return Task.CompletedTask;
        }
    }
}
