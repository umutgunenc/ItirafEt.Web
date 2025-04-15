using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{
    public class ReactionType
    {
               
        [Key]
        public int Id { get; private set; }

        [Required, MaxLength(16)]
        public string Name { get; private set; }

    }
}
