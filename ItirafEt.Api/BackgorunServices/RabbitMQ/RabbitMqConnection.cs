using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static ItirafEt.Api.BackgorunServices.RabbitMQ.RabbitMqConnection;

namespace ItirafEt.Api.BackgorunServices.RabbitMQ
{

    public class RabbitMqConnection : IAsyncDisposable
    {
        private IConnection? _connection;
        private readonly IConfiguration _configuration;
        private readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);
        private bool _disposed = false;



        public RabbitMqConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IConnection> GetConnectionAsync()
        {

            try
            {
                await _sync.WaitAsync();

                if (_connection != null && _connection.IsOpen)
                    return _connection;

                await CreateConnectionAsync();

                return _connection;
            }
            finally
            {
                _sync.Release();
            }
            
        }

        private async Task CreateConnectionAsync()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration.GetValue<string>("RabbitMQ:Uri"))
            };
            _connection = await factory.CreateConnectionAsync();

            if (_connection != null && _connection.IsOpen)
                _disposed = false;
        }

        public async ValueTask DisposeAsync()
        {
            await _sync.WaitAsync();

            try
            {
                if (_disposed) return;
                _disposed = true;
                if (_connection != null)
                {
                    if (_connection.IsOpen)
                        await _connection.CloseAsync();

                    await _connection.DisposeAsync();
                    _connection = null;
                }
            }
            finally
            {
                _sync.Release();
                _sync.Dispose();
            }
        }
    }
}

