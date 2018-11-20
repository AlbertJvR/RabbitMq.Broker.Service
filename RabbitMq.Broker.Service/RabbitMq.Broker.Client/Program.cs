using Microservice.Abstractions;

namespace RabbitMq.Broker.Client
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
