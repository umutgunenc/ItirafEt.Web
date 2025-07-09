using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class User
    {
        public User()
        {
            Posts = new HashSet<Post>();
            PostHistories = new HashSet<PostHistory>();
            PostReactions = new HashSet<PostReaction>();
            Comments = new HashSet<Comment>();
            CommentHistories = new HashSet<CommentHistory>();
            CommentReactions = new HashSet<CommentReaction>();
            ConversationsInitiated = new HashSet<Conversation>();
            ConversationsReceived = new HashSet<Conversation>();
            MessageReactions = new HashSet<MessageReaction>();
            SentMessages = new HashSet<Message>();
            BlockedUsers = new HashSet<UserBlock>();
            BlockedByUsers = new HashSet<UserBlock>();
            ReadPosts = new HashSet<UserReadPost>();
        }
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string UserName { get; set; }
        public string? ProfilePictureUrl { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public DateTime? BannedDate { get; set; } 
        public DateTime? BannedDateUntil { get; set; } 

        [ForeignKey(nameof(AdminastorUserId))]
        public User AdminastorUser { get; set; }
        public Guid? AdminastorUserId { get; set; } 

        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsBanned { get; set; }

        [Required]
        public bool IsProfilePrivate { get; set; }

        [Required]
        public bool IsTermOfUse { get; set; }

        [Required]
        public string RoleName { get; set; }

        [ForeignKey(nameof(RoleName))]
        public virtual RoleType Role { get; set; }

        [Required]
        public int GenderId { get; set; }

        [ForeignKey(nameof(GenderId))]
        public virtual GenderType Gender { get; set; }


        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<PostHistory> PostHistories { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<CommentHistory> CommentHistories { get; set; }
        public virtual ICollection<PostReaction> PostReactions { get; set; }
        public virtual ICollection<CommentReaction> CommentReactions { get; set; }
        public virtual ICollection<Conversation> ConversationsInitiated { get; set; }  // User1 
        public virtual ICollection<Conversation> ConversationsReceived { get; set; }   // User2 
        public virtual ICollection<MessageReaction> MessageReactions { get; set; }
        public virtual ICollection<MessageReport> MessageReports { get; set; }
        public virtual ICollection<CommentReport> CommentReports { get; set; }
        public virtual ICollection<PostReport> PostReports { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<UserBlock> BlockedUsers { get; set; } // Bu kullanıcı kimi engelledi
        public virtual ICollection<UserBlock> BlockedByUsers { get; set; } // Bu kullanıcı kim tarafından engellendi
        public virtual ICollection<UserReadPost> ReadPosts { get; set; } 
    }
}
