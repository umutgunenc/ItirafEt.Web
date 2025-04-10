using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{
    public class RoleType
    {
        protected RoleType()
        {
            Users = new HashSet<User>();
        }

        public static readonly RoleType SuperAdmin = new RoleType((int)UserRole.SuperAdmin, nameof(UserRole.SuperAdmin));
        public static readonly RoleType Admin = new RoleType((int)UserRole.Admin, nameof(UserRole.Admin));
        public static readonly RoleType Moderator = new RoleType((int)UserRole.Moderator, nameof(UserRole.Moderator));
        public static readonly RoleType SuperUser = new RoleType((int)UserRole.SuperUser, nameof(UserRole.SuperUser));
        public static readonly RoleType User = new RoleType((int)UserRole.User, nameof(UserRole.User));

        public static IEnumerable<RoleType> List() => new[]
        {
            SuperAdmin,
            Admin,
            Moderator,
            User
        };

        public static RoleType FromEnum(UserRole ur) =>
            List().Single(x => x.Id == (int)ur);

        [Key]
        public int Id { get; private set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; private set; }

        public virtual ICollection<User> Users { get; private set; }

        private RoleType(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }


    }

}
