using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.ViewModels
{
    public class UserPostsViewModel
    {
        public bool HasNextPage { get; set; }
        public int TotalCount { get; set; }
        public string UserName { get; set; }
        public string? UserProfilePicture { get; set; } 
        public List<ListOfUserPost> UserPosts { get; set; } = new List<ListOfUserPost>();
    }

    public class  ListOfUserPost
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostContentReview { get; set; }
        public int PostViewCount { get; set; }
        public int PostLikeCount { get; set; }
        public DateTime PostCreatedDate { get; set; }
    }
}
