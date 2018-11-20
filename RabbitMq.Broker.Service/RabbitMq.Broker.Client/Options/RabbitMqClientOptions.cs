using Newtonsoft.Json;
using System.ComponentModel;

namespace RabbitMq.Broker.Client.Options
{
    public class RabbitMqClientOptions
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(2000)]
        public int TimeoutMilliseconds { get; set; }
        public string SubscribeQueueName { get; set; }
        public string PublishQueueName { get; set; }
    }
}
