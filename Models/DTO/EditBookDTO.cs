using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.DTO
{
    public class EditBookDTO
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 10)] // ISBN can be 10 or 13
        public string Isbn { get; set; }

        public DateTime PublishedDate { get; set; }

        [Range(1, 1000)]
        public int Stock { get; set; }

        [Range(0, 1000)]
        public int Available { get; set; }

        // 编辑书籍时，我们只需要作者和出版社的 ID
        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int PublisherId { get; set; }
    }
}
