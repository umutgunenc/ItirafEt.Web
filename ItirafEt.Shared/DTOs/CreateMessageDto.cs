using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ItirafEt.Shared.DTOs
{
    public class CreateMessageDto
    {
        public string? ConversationId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }

        [JsonIgnore] // Bunları client göndermez, sunucu ekler
        public string SenderIpAddress { get; set; }

        [JsonIgnore]
        public string SenderDeviceInfo { get; set; }

        public string? PhotoUrl { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
