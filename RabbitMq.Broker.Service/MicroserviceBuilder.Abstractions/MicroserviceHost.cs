using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Abstractions
{
    public class MicroserviceHost:IMicroserviceHost
    {
        private readonly ServiceProvider _serviceProvider;

        public MicroserviceHost(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void Run()
        {
            _serviceProvider.GetService<IStartup>().Run().Wait();
        }
    }
}
