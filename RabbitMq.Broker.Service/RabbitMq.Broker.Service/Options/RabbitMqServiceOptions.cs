using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMq.Broker.Service.Options
{
    public class RabbitMqServiceOptions
    {
        public string SubscribeQueueName { get; set; }
        public string PublishQueueName { get; set; }
    }
}
