using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ItirafEt.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Data.Entities
{
    public class ReportType
    {

        public ReportType()
        {
            Reports = new HashSet<Report>();
        }


        [Key]
        public int Id { get; set; }

        [Required, MaxLength(64)]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public ReportClassEnum ReportClass{ get; set; }

        [MaxLength(128)]
        public string? IconUrl { get; set; }

        public virtual ICollection<Report> Reports { get; set; }

    }



}
