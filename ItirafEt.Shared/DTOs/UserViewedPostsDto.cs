using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public record UserViewedPostsDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostCreatorUserName { get; set; }
        public int PostCreatorUserGenderId { get; set; }
        public int PostCreatorUserAge { get; set; }
        public Guid PostCreatorUserId { get; set; }
        public string? PostCreatorUserProfileImageUrl { get; set; }
        public DateTime ReadDate { get; set; }
    }
}
