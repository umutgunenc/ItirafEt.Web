using System.ComponentModel.DataAnnotations.Schema;

namespace ItirafEt.Api.Data.Entities
{
    public class UserReadPost
    {
        public Guid UserId { get; set; }
        public int PostId { get; set; }

        public DateTime ReadDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }
    }
}
