using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public class CommentsDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Lütfen yorum alanını doldurduktan sonra tekrar deneyin.")]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ParentCommentId { get; set; }
        public int? CommentCount { get; set; }
        public int? LikeCount { get; set; }
        public int? DislikeCount { get; set; }
        public int? ReportCount { get; set; }
        public string DeviceInfo{ get; set; }
        public string IpAddress { get; set; }
        public List<CommentsDto>? CommentReplies { get; set; }
        public List<ReactionDto>? CommentRections { get; set; }

    }
}
