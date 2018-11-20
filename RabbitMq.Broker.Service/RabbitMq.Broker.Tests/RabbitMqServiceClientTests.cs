using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMq.Adapter.Abstractions;
using RabbitMq.Broker.Client;
using RabbitMq.Broker.Client.Options;
using RabbitMq.Broker.Service;
using RabbitMq.Broker.Service.Options;
using Xunit;

namespace RabbitMq.Broker.Tests
{
    public class RabbitMqServiceClientTests
    {
        private Mock<IOptions<RabbitMqOptions>> _mockOptions;
        private ServiceProvider _sp;

        public RabbitMqServiceClientTests()
        {
            var sc = new ServiceCollection();
            sc.AddLogging();
            sc.AddOptions();
            sc.Configure<RabbitMqOptions>(cfg => { cfg.HostName = "localhost"; });
            sc.Configure<RabbitMqServiceOptions>(cfg =>
            {
                cfg.SubscribeQueueName = "Greeting.Service";
                cfg.PublishQueueName = "Greeting.Client";
            });
            sc.Configure<RabbitMqClientOptions>(cfg =>
            {
                cfg.SubscribeQueueName = "Greeting.Client";
                cfg.PublishQueueName = "Greeting.Service";
                cfg.TimeoutMilliseconds = 20;
            });
            sc.AddSingleton<MockRabbitMqQueue>();
            sc.AddSingleton<IRabbitMqAdapter, MockRabbitMqAdapter>();
            sc.AddSingleton<RabbitMqNameListenerService>();
            sc.AddSingleton<RabbitMqNameSenderClient>();
            _sp = sc.BuildServiceProvider();
        }
        [Fact]
        public async Task TestValueSendAndReceive()
        {
            var server = _sp.GetService<RabbitMqNameListenerService>();
            var client = _sp.GetService<RabbitMqNameSenderClient>();
            await server.ListenToMessages();
            Assert.True(await client.SendName("Hello my name is, ", "Albert"));
        }
        [Fact]
        public async Task TestValueSendAndTimeout()
        {
            var client = _sp.GetService<RabbitMqNameSenderClient>();
            Assert.False(await client.SendName("Hello my name is, ", "Albert"));
        }
    }
}
