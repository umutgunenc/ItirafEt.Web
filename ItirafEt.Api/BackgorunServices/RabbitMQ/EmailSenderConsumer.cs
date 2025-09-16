using ItirafEt.Api.EmailServices;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;
using System.Text.Json;
using ItirafEt.Api.ConstStrings;

namespace ItirafEt.Api.BackgorunServices.RabbitMQ
{
    public class EmailSenderConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private IConnection? _connection;
        private IChannel? _channel;

        public EmailSenderConsumer(IConfiguration configuration, IEmailSender emailSender)
        {
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration.GetValue<string>("RabbitMQ:Uri"))
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("email-exchange", ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: "email-service-queue",
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync("email-service-queue", "email-exchange", EmailTypes.Welcome);
            await _channel.QueueBindAsync("email-service-queue", "email-exchange", EmailTypes.Reset);
            await _channel.QueueBindAsync("email-service-queue", "email-exchange", EmailTypes.Ban);
            await _channel.QueueBindAsync("email-service-queue", "email-exchange", EmailTypes.ActivateAccount);
            await _channel.QueueBindAsync("email-service-queue", "email-exchange", EmailTypes.AccountBlocked);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, e) =>
            {
                try
                {
                    var body = e.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<EmailMessageDto>(json);

                    if (message != null)
                    {
                        await _emailSender.SendEmailAsync(message.To, message.Subject, message.Body);
                        await _channel.BasicAckAsync(e.DeliveryTag, multiple: false);
                    }
                    else
                        await _channel.BasicNackAsync(e.DeliveryTag, multiple: false, requeue: false);
                }
                catch (Exception)
                {
                    await _channel.BasicNackAsync(e.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "email-service-queue",
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync(cancellationToken);

            if (_connection != null)
                await _connection.CloseAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
