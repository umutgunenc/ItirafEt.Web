using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Services.ClientServices.NewFolder;

namespace ItirafEt.Shared.ViewModels
{
    public class UserPostsViewModel : PageResult<ListOfUserPost>
    {
        public string UserName { get; set; }
        public string? UserProfilePicture { get; set; }

    }

    public class ListOfUserPost
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostContentReview { get; set; }
        public int PostViewCount { get; set; }
        public int PostLikeCount { get; set; }
        public DateTime PostCreatedDate { get; set; }
        public DateTime? PostUpdatedDate { get; set; }
        public bool IsDeletedByUser { get; set; }
        public bool IsDeletedByAdmin { get; set; }
    }
}
