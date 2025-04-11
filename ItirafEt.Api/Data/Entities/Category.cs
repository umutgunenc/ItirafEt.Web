using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Data.Entities
{
    public class Category
    {
        public Category()
        {
            Posts = new HashSet<Post>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string CategoryName { get; set; }

        [Required]
        public bool isActive { get; set; }
        public int CategoryOrder { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
