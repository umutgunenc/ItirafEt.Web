
using ItirafEt.Api.ConstStrings;
using ItirafEt.Api.EmailServices;
using ItirafEt.Api.Models;
using ItirafEt.Shared.ViewModels;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ItirafEt.Api.BackgorunServices.RabbitMQ
{
    public class MessageSenderProducer : IAsyncDisposable
    {

        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;

        public MessageSenderProducer(IConfiguration configuration)
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
            await _channel.ExchangeDeclareAsync("message-send-exchange", ExchangeType.Direct, durable: true);

            // Queue
            await _channel.QueueDeclareAsync(
                queue: "message-service-queue",
                durable: true,
                exclusive: false,
                autoDelete: false);

            // Queue bind 
            await _channel.QueueBindAsync("message-service-queue", "message-send-exchange", MessageTypes.SendMessage);


        }

        /// <summary>
        /// Mesajı RabbitMQ'ya gönderir.
        /// routingKey MessageType'ten bir değer olmalı
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync(string routingKey, RabbitMqMessageViewModel message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var props = new BasicProperties()
                {
                    DeliveryMode = DeliveryModes.Persistent // mesaj kaybolmaz
                };

                await _channel.BasicPublishAsync(
                    exchange: "message-send-exchange",
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: props,
                    body: body);

                Console.WriteLine($"Message sent to RabbitMQ: {json}");

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
           
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
