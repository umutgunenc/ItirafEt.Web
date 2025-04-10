using System.ComponentModel.DataAnnotations;

namespace ItirafEt.Api.Data.Entities
{
    public class MessageAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MessageId { get; set; }

        [Required]
        public string PhotoUrl { get; set; }

        public virtual Message Message { get; set; }
    }
}
