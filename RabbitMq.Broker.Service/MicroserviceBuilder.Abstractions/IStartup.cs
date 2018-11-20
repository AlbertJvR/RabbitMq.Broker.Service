using System.Threading.Tasks;

namespace Microservice.Abstractions
{
    public interface IStartup
    {
        Task Run();
    }
}