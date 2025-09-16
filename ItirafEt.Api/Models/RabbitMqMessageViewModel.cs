using ItirafEt.Shared.ViewModels;

namespace ItirafEt.Api.Models
{
    public class RabbitMqMessageViewModel :MessageViewModel
    {
        public string? SenderUserProfileImageUrl { get; set; }
    }
}
