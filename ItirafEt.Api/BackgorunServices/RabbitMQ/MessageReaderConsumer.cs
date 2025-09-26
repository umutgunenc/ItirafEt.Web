using ItirafEt.Api.ConstStrings;
using ItirafEt.Api.HubServices;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ItirafEt.Api.Models;
using ItirafEt.Shared.ViewModels;

namespace ItirafEt.Api.BackgorunServices.RabbitMQ
{
    public class MessageReaderConsumer : BackgroundService
    {
        private IChannel? _channel;
        private RabbitMqConnection _rabbitMqConnection;
        private readonly MessageHubService _hubService;

        public MessageReaderConsumer(MessageHubService hubService, RabbitMqConnection rabbitMqConnection)
        {
            _hubService = hubService;
            _rabbitMqConnection = rabbitMqConnection;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var connection = await _rabbitMqConnection.GetConnectionAsync();
            _channel = await connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("message-exchange", ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(MessageTypes.ReadMessage, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(MessageTypes.ReadMessage, "message-exchange", MessageTypes.ReadMessage);

            await _channel.BasicQosAsync(0, 10, false, cancellationToken);


            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, e) =>
            {
                _ = Task.Run(async () => {

                    try
                    {

                        var json = Encoding.UTF8.GetString(e.Body.ToArray());
                        var message = JsonSerializer.Deserialize<MessageViewModel>(json);

                    if (message != null)
                    {
                        // sadece okundu bilgisini karşı tarafa ilet

                        await _hubService.ReadMessageAsync(message.ConversationId, message);
                        await _hubService.MessageReadByCurrentUserAsync(message.ReceiverId, message.ConversationId);


                        await _channel.BasicAckAsync(e.DeliveryTag, false);
                    }
                    else
                        await _channel.BasicNackAsync(e.DeliveryTag, false, false);
                }
                catch
                {
                    await _channel.BasicNackAsync(e.DeliveryTag, false, true);
                }
            }, stoppingToken);

        };

            await _channel.BasicConsumeAsync(MessageTypes.ReadMessage, autoAck: false, consumer, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync(cancellationToken);

            //if (_connection != null)
            //    await _connection.CloseAsync(cancellationToken);

            await StopAsync(cancellationToken);
        }
    }
}
