using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Kategori Adı Boş Olamaz.")]
        [MaxLength(64,ErrorMessage ="Kategori Adı Maksimum 64 Karakter Uzunluğunda Olabilir.")]
        public string CategoryName { get; set; }

        [Required]
        public bool isActive { get; set; }

        [Required(ErrorMessage = "Sıra Numarası Boş Olamaz.")]
        [Range(1,99,ErrorMessage ="Sıra Numarası 1 ile 100 arasında olmalıdır.")]
        public int? CategoryOrder { get; set; }


        [Required(ErrorMessage = "Kategori Iconu Boş Olamaz")]
        public string CategoryIconUrl { get; set; }

        public int PostCount { get; set; }


    }
}
