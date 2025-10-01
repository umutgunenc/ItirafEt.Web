using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class UserRoles
    {
        public UserRoles()
        {
                
        }

        public Guid UserId { get; set; }
        public User User { get; set; }

        [MaxLength(64)]

        public string RoleName { get; set; }
        public RoleType Role { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public Guid AssignedByUserId { get; set; }
        public User AssignedByUser { get; set; }

    }
}
