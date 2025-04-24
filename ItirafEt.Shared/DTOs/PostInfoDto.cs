using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public record PostInfoDto
    {
        public int PostId { get; set; }
        public string? PostTitle { get; set; }
        public string? PostCreatorUserName { get; set; }
        public string? PostContentReview { get; set; }
        public int PostViewCount { get; set; }
        public int PostLikeCount { get; set; }
        public DateTime PostCreatedDate { get; set; }
        public bool IsPostReadByUser { get; set; }
    }
}
