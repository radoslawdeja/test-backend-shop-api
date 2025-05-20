namespace Test.Shop.Infrastructure.Shared.RabbitMq.Abstractions
{
    public interface IRabbitProducer
    {
        void Publish(object @event, string queueName);
    }
}
