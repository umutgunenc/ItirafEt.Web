using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{

    public class GenderType
    {
        protected GenderType()
        {
            Users = new HashSet<User>();
        }

        public static readonly GenderType Unknown = new GenderType((int)Gender.Unknown, nameof(Gender.Unknown));
        public static readonly GenderType Male = new GenderType((int)Gender.Male, nameof(Gender.Male));
        public static readonly GenderType Female = new GenderType((int)Gender.Female, nameof(Gender.Female));
        public static readonly GenderType Other = new GenderType((int)Gender.Other, nameof(Gender.Other));

        public static IEnumerable<GenderType> List() => new[]
        {
            Unknown,
            Male,
            Female,
            Other
        };

        public static GenderType FromEnum(Gender g) =>
            List().Single(x => x.Id == (int)g);

        [Key]
        public int Id { get; private set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; private set; }

        public virtual ICollection<User> Users { get; private set; }

        private GenderType(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }
    }
}
