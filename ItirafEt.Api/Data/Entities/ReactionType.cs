using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{
    public class ReactionType
    {
        public ReactionType()
        {
            PostReactions = new HashSet<PostReaction>();
            MessageReactions = new HashSet<MessageReaction>();
            CommentReactions = new HashSet<CommentReaction>();
        }
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(16)]
        public string Name { get; set; }

        public virtual ICollection<PostReaction> PostReactions { get; set; } 
        public virtual ICollection<MessageReaction> MessageReactions { get; set; } 
        public virtual ICollection<CommentReaction> CommentReactions { get; set; } 


    }
}
