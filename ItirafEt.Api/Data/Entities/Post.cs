using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
            PostReactions = new HashSet<PostReaction>();
            Readers = new HashSet<UserReadPost>();
            Reports = new HashSet<Report>();
        }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(256)]
        public string Title { get; set; }

        [Required, MaxLength(4096)]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
        [Required]
        public Guid UserId { get; set; } 

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public bool IsDeletedByUser { get; set; }

        [Required]
        public bool IsDeletedByAdmin { get; set; }

        [Required, MaxLength(45)]
        public string IpAddress { get; set; }

        [MaxLength(512)]
        public string DeviceInfo { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } 
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<PostReaction> PostReactions { get; set; }
        public virtual ICollection<UserReadPost> Readers { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

    }
}
