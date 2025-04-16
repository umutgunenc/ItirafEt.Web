using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItirafEt.Shared.DTOs
{
    public record PostDto
    {

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UserName { get; set; }
        public int CommentCount { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }

        [Required(ErrorMessage = "Lütfen Başlık Giriniz.")]
        [MaxLength(256, ErrorMessage = "Başlık Maksimum 256 Karakter Uzunluğunda Olabilir.")]
        [MinLength(10, ErrorMessage = "Başlık en az 10 karakter olmalıdır.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İçerik alanı boş bırakılamaz.")]
        [MaxLength(4096, ErrorMessage = "İçerik en fazla 4096 karakter olabilir.")]
        [MinLength(100, ErrorMessage = "İçerik en az 100 karakter olmalıdır.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Lütfen Kategori Seçiniz.")]
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }

        public string DeviceInfo { get; set; }
        public string IpAddress { get; set; }

    }
}
