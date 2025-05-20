using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Test.Shop.Infrastructure.Shared.RabbitMq.Models;

namespace Test.Shop.Infrastructure.Shared.RabbitMq
{
    public abstract class RabbitBaseConsumer : RabbitBaseClient
    {
        private readonly ILogger<RabbitBaseConsumer> _logger;

        protected RabbitBaseConsumer(ConnectionFactory connectionFactory, ILogger<RabbitBaseConsumer> consumerLogger,
            RabbitSettings rabbitSettings, ILogger<RabbitBaseClient> baseLogger) : base(connectionFactory, baseLogger, rabbitSettings)
        {
            _logger = consumerLogger;
        }

        protected virtual async Task OnEventReceived<T>(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var body = Encoding.UTF8.GetString(@event.Body.ToArray());
                var message = JsonConvert.DeserializeObject<T>(body);
                await Console.Out.WriteLineAsync(message!.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while retrieving message from queue.");
            }
            finally
            {
                Channel!.BasicAck(@event.DeliveryTag, false);
            }
        }
    }
}
