using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public class CommentsDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }        
        public int? ParentCommentId { get; set; }
        public int? CommentCount { get; set; }
        public int? LikeCount { get; set; }
        public int? DislikeCount { get; set; }
        public int? ReportCount { get; set; }
        public List<CommentsDto>? CommentReplies { get; set; }
        public List<ReactionDto>? CommentRections { get; set; }

    }
}
