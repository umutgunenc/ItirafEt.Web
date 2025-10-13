using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ItirafEt.Api.Data.Entities
{

    [Index(nameof(Name), IsUnique = true)]  // Class üstüne konur
    public class ReportClass
    {
        public ReportClass()
        {
            ReportTypes = new HashSet<ReportType>();
        }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(64)]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [MaxLength(128)]
        public string? IconUrl { get; set; }
        public virtual ICollection<ReportType> ReportTypes { get; set; }

    }
}
