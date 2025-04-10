using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class Comment
    {
        public Comment()
        {
             Replies = new HashSet<Comment>();
             CommentReactions = new HashSet<CommentReaction>();  
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(4096)]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(512)]
        public string DeviceInfo { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public int? ParentCommentId { get; set; } 

        [ForeignKey(nameof(ParentCommentId))]
        public virtual Comment ParentComment { get; set; } 

        public virtual ICollection<Comment> Replies { get; set; }
        public virtual ICollection<CommentReaction> CommentReactions { get; set; }

    }
}