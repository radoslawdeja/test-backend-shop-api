using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Test.Shop.Infrastructure.Shared.RabbitMq.Models;

namespace Test.Shop.Infrastructure.Shared.RabbitMq
{
    public abstract class RabbitBaseClient : IDisposable
    {
        protected IModel? Channel { get; private set; }
        private IConnection? _connection;
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitBaseClient> _logger;
        protected readonly RabbitSettings _settings;

        protected RabbitBaseClient(ConnectionFactory connectionFactory, ILogger<RabbitBaseClient> logger, RabbitSettings settings)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _settings = settings;
            ConnectToRabbit();
        }

        private void ConnectToRabbit()
        {
            if (_connection == null || Channel?.IsOpen == false)
            {
                _connection = _connectionFactory.CreateConnection(_settings.Endpoints);
            }

            if (Channel == null || Channel?.IsOpen == false)
            {
                Channel = _connection.CreateModel();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    Channel?.Close();
                    Channel?.Dispose();
                    Channel = null!;

                    _connection?.Close();
                    _connection?.Dispose();
                    _connection = null!;
                    _logger.LogInformation("RabbitMQ connection is closed.");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Cannot dispose RabbitMQ channel or connection");
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}