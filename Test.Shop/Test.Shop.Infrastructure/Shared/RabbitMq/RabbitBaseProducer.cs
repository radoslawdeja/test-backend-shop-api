using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Test.Shop.Infrastructure.Shared.RabbitMq.Abstractions;
using Test.Shop.Infrastructure.Shared.RabbitMq.Models;

namespace Test.Shop.Infrastructure.Shared.RabbitMq
{
    public abstract class RabbitBaseProducer : RabbitBaseClient, IRabbitProducer
    {
        private readonly ILogger<RabbitBaseProducer> _producerBaseLogger;
        private readonly RabbitSettings _rabbitSettings;

        protected RabbitBaseProducer(ConnectionFactory connectionFactory, ILogger<RabbitBaseClient> logger,
            ILogger<RabbitBaseProducer> producerBaseLogger, RabbitSettings rabbitSettings) : base(connectionFactory, logger, rabbitSettings)
        {
            _producerBaseLogger = producerBaseLogger;
            _rabbitSettings = rabbitSettings;
            _producerBaseLogger.LogInformation("RabbitProducer is Working");
        }

        public void Publish(object @event, string queueName)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
                var properties = Channel!.CreateBasicProperties();
                properties.UserId = _rabbitSettings.Username;
                properties.Persistent = false;
                Channel.BasicPublish(string.Empty, queueName, properties, body);
            }
            catch (Exception ex)
            {
                _producerBaseLogger.LogCritical(ex, "Error while publishing");
            }
        }
    }
}
