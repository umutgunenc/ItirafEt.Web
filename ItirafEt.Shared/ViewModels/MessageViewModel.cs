using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Refit;

namespace ItirafEt.Shared.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public Guid ConversationId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string SenderUserName { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeletedBySender { get; set; }
        public bool IsDeletedByReceiver { get; set; }
        public string? SenderIpAddress { get; set; }
        public string? SenderDeviceInfo { get; set; }
        public string? PhotoId { get; set; }
        public string? ThumbnailId { get; set; }
        public string? SignedPhotoUrl { get; set; }
        public string? SignedThumbnailUrl { get; set; }
        public IFormFile? Photo { get; set; }

    }
}
