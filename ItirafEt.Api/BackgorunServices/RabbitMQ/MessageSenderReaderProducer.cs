
using ItirafEt.Api.ConstStrings;
using ItirafEt.Api.EmailServices;
using ItirafEt.Api.Models;
using ItirafEt.Shared.ViewModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace ItirafEt.Api.BackgorunServices.RabbitMQ
{
    public class MessageSenderReaderProducer : IAsyncDisposable
    {

        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;

        public MessageSenderReaderProducer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InitAsync()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration.GetValue<string>("RabbitMQ:Uri"))
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Exchange 
            await _channel.ExchangeDeclareAsync("message-exchange", ExchangeType.Direct, durable: true);

            // Queue
            await _channel.QueueDeclareAsync(MessageTypes.SendMessage, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueDeclareAsync(MessageTypes.ReadMessage, durable: true, exclusive: false, autoDelete: false);


            // Queue bind 
            await _channel.QueueBindAsync(MessageTypes.SendMessage, "message-exchange", MessageTypes.SendMessage);
            await _channel.QueueBindAsync(MessageTypes.ReadMessage, "message-exchange", MessageTypes.ReadMessage);

        }

        /// <summary>
        /// Generic publish method
        /// routingKey -> hangi işlem (SendMessage, ReadMessage)
        /// message -> her tipte olabilir
        /// </summary>
        public async Task PublishAsync<T>(string routingKey, T message)
        {

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties()
            {
                DeliveryMode = DeliveryModes.Persistent // mesaj kaybolmaz
            };

            await _channel.BasicPublishAsync(
                exchange: "message-exchange",
                routingKey: routingKey,
                mandatory: true,
                basicProperties: props,
                body: body);

        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}
