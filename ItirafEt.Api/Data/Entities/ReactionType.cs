using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{
    public class ReactionType
    {
        protected ReactionType()
        {
            Users = new HashSet<User>();
        }

        public static readonly ReactionType Like = new ReactionType((int)Reaction.Like, nameof(Reaction.Like));
        public static readonly ReactionType Dislike = new ReactionType((int)Reaction.Dislike, nameof(Reaction.Dislike));
        public static readonly ReactionType Report = new ReactionType((int)Reaction.Report, nameof(Reaction.Report));

        public static IEnumerable<ReactionType> List() => new[]
        {
            Like,
            Dislike,
            Report
        };

        public static ReactionType FromEnum(Reaction r) =>
            List().Single(x => x.Id == (int)r);

        [Key]
        public int Id { get; private set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; private set; }

        public virtual ICollection<User> Users { get; private set; }

        private ReactionType(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }

    }
}
