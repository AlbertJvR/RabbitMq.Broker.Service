using System;
using Microservice.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RabbitMq.Broker.Service
{
    class Program
    {

        public static void Main(string[] args)
        {
            CreateMicroserviceBuilder(args).Build().Run();
        }

        public static MicroserviceBuilder CreateMicroserviceBuilder(string[] args) =>
            MicroserviceBuilder.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
