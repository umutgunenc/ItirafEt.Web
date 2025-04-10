using System.ComponentModel.DataAnnotations;
using ItirafEt.Shared.Enums;

namespace ItirafEt.Api.Data.Entities
{
    public class ReportType
    {
        //protected ReportType()
        //{
        //    Users = new HashSet<User>();
        //}

        public static readonly ReportType OffTopic = new ReportType((int)Report.OffTopic, nameof(Report.OffTopic));
        public static readonly ReportType InappropriateContent = new ReportType((int)Report.InappropriateContent, nameof(Report.InappropriateContent));
        public static readonly ReportType AbusiveLanguage = new ReportType((int)Report.AbusiveLanguage, nameof(Report.AbusiveLanguage));
        public static readonly ReportType Harassment = new ReportType((int)Report.Harassment, nameof(Report.Harassment));
        public static readonly ReportType Spam = new ReportType((int)Report.Spam, nameof(Report.Spam));


        public static IEnumerable<ReportType> List() => new[]
        {
            OffTopic,
            InappropriateContent,
            AbusiveLanguage,
            Harassment,
            Spam
        };

        public static ReportType FromEnum(Report r) =>
            List().Single(x => x.Id == (int)r);

        [Key]
        public int Id { get; private set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; private set; }

        //public virtual ICollection<User> Users { get; private set; }

        private ReportType(int id, string name) 
        {
            Id = id;
            Name = name;
        }


    }
}
