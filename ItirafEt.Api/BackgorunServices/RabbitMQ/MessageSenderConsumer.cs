using ItirafEt.Api.ConstStrings;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading.Channels;
using System.Text;
using ItirafEt.Api.Models;
using System.Text.Json;
using ItirafEt.Shared.ViewModels;
using ItirafEt.Api.HubServices;
using System.Data;

namespace ItirafEt.Api.BackgorunServices.RabbitMQ
{
    public class MessageSenderConsumer : BackgroundService
    {
        private IChannel? _channel;
        private RabbitMqConnection _rabbitMqConnection;
        private readonly MessageHubService _hubService;

        public MessageSenderConsumer(MessageHubService hubService, RabbitMqConnection rabbitMqConnection)
        {
            _hubService = hubService;
            _rabbitMqConnection = rabbitMqConnection;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            var connection = await _rabbitMqConnection.GetConnectionAsync();
            _channel = await connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync("message-exchange", ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(MessageTypes.SendMessage, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(MessageTypes.SendMessage, "message-exchange", MessageTypes.SendMessage);

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
                    var message = JsonSerializer.Deserialize<RabbitMqMessageViewModel>(json);


                    if (message != null)
                    {

                        var messageViewModel = new MessageViewModel
                        {
                            Id = message.Id,
                            Content = message.Content,
                            CreatedDate = message.CreatedDate,
                            SenderId = message.SenderId,
                            SenderUserName = message.SenderUserName,
                            ConversationId = message.ConversationId,
                            PhotoUrl = message.PhotoUrl,
                        };

                        var inboxViewModel = new InboxItemViewModel
                        {
                            ConversationId = messageViewModel.ConversationId,
                            LastMessageDate = messageViewModel.CreatedDate,
                            LastMessagePrewiew = messageViewModel.Content,
                            SenderUserUserName = messageViewModel.SenderUserName,
                            SenderUserProfileImageUrl = message.SenderUserProfileImageUrl,
                            UnreadMessageCount = 1
                        };

                        await _hubService.SendMessageAsync(message.ConversationId, messageViewModel);
                        await _hubService.SendMessageNotificationAsync(message.ConversationId, messageViewModel);
                        await _hubService.NewMessageForInboxAsync(message.ReceiverId, inboxViewModel);

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
                queue: MessageTypes.SendMessage,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
                await _channel.CloseAsync(cancellationToken);

            //if (_connection != null)
            //    await _connection.CloseAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
