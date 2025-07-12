using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.Models
{
    public static class HubConstants
    {
        public const string CategoryHub = "/categoryhub";
        public const string ReactionHub = "/reactionhub";
        public const string CommentHub = "/commenthub";
        public const string PostViewHub = "/postviewhub";
        public const string MessageHub = "/messagehub";

        public enum HubType
        {
            Category,
            Reaction,
            Comment,
            PostView,
            Message
        }

        public static string GetHubUrl(HubType hubType)
        {
            return hubType switch
            {
                HubType.Category => CategoryHub,
                HubType.Reaction => ReactionHub,
                HubType.Comment => CommentHub,
                HubType.PostView => PostViewHub,
                HubType.Message => MessageHub,
                _ => throw new ArgumentOutOfRangeException(nameof(hubType), hubType, null)
            };
        }
    }
}
