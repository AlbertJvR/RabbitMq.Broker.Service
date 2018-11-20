using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Broker.Abstractions
{
    public interface IRabbitMqAdapter
    {
        Task SendMessage(string message);
    }
}
