using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public class ReactionDto
    {
        public int Id { get; set; }

        public Guid ReactingUserId { get; set; }

        public string ReactingUserUserName { get; set; }

        public int ReactionTypeId { get; set; }
        public int ReactingUserAge { get; set; }
        public int ReactingUserGenderId { get; set; }
        public string ReactingUserProfileImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? CommentId { get; set; }
        public int? PostId { get; set; }
    }
}
