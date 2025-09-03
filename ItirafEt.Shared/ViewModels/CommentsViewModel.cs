using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class CommentsViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }

        [Required(ErrorMessage= "Lütfen yorumunuzu yazın.")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ParentCommentId { get; set; }
        public int? CommentCount { get; set; }
        public int? LikeCount { get; set; }
        public int? DislikeCount { get; set; }
        public int? ReportCount { get; set; }
        public bool AnyReplies { get; set; }
        public string? CommentUserProfilPhotoUrl { get; set; }

        [JsonIgnore]
        public string DeviceInfo{ get; set; }

        [JsonIgnore]
        public string IpAddress { get; set; }
        public List<CommentsViewModel>? CommentReplies { get; set; }
        public List<ReactionViewModel>? CommentRections { get; set; }

    }
}
