namespace Test.Shop.Infrastructure.Shared.RabbitMq.Models
{
    public class RabbitSettings
    {
        public List<string?>? Endpoints { get; set; }
        public string? VirtualHost { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int Port { get; set; }
        public string? Exchange { get; set; }
    }
}
