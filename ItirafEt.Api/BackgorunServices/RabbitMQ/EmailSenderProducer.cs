using System.Net.Mail;
using System.Text;
using System.Text.Json;
using ItirafEt.Api.EmailServices;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;

namespace ItirafEt.Api.BackgorunServices.RabbitMQ
{
    public class EmailSenderProducer : IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;

        public EmailSenderProducer(IConfiguration configuration)
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
            await _channel.ExchangeDeclareAsync("email-exchange", ExchangeType.Direct, durable: true);

            // Queue
            await _channel.QueueDeclareAsync(
                queue: "email-service-queue",
                durable: true,
                exclusive: false,
                autoDelete: false);

            // Queue bind 
            await _channel.QueueBindAsync("email-service-queue", "email-exchange", EmailTypes.Welcome);
            await _channel.QueueBindAsync("email-service-queue", "email-exchange", EmailTypes.Reset);
            await _channel.QueueBindAsync("email-service-queue", "email-exchange", EmailTypes.Ban);
        }

        /// <summary>
        /// Email mesajını RabbitMQ'ya gönderir.
        /// routingKey EmailtTypes'ten bir değer olmalı (welcome, reset, ban,...)
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync(string routingKey, EmailMessageDto message)
        {

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties()
            {
                DeliveryMode = DeliveryModes.Persistent // mesaj kaybolmaz
            };

            await _channel.BasicPublishAsync(
                exchange: "email-exchange",
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
