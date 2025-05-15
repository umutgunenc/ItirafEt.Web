using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public record PostViewersDto
    {
        public string PostViewerUserName { get; set; }
        public Guid PostViewerUserId { get; set; }
        public string? PostViewerUserProfileImageUrl { get; set; }
        public int PostViewerGenderId { get; set; }
        public int PostViewerAge { get; set; }
        public DateTime ReadDate { get; set; }


    }
}
