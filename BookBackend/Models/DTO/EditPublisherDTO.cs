using System.ComponentModel.DataAnnotations;

namespace book_backend.Models.DTO
{
    public class EditPublisherDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // 出版社名称
    }
}
