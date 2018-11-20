using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.Abstractions
{
    public interface IMicroserviceStartup
    {
        void AddConfig(IConfiguration configuration);
        void ConfigureServices(IServiceCollection services);
    }
}