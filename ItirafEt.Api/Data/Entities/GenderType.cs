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

        public static readonly GenderType Unknown = new GenderType((int)GenderEnum.Unknown, nameof(GenderEnum.Unknown));
        public static readonly GenderType Male = new GenderType((int)GenderEnum.Male, nameof(GenderEnum.Male));
        public static readonly GenderType Female = new GenderType((int)GenderEnum.Female, nameof(GenderEnum.Female));
        public static readonly GenderType Other = new GenderType((int)GenderEnum.Other, nameof(GenderEnum.Other));

        public static IEnumerable<GenderType> List() => new[]
        {
            Unknown,
            Male,
            Female,
            Other
        };

        public static GenderType FromEnum(GenderEnum g) =>
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
