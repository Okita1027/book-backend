using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.DTO
{
    public class EditBookDTO
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 10)] // ISBN最小10，最大13
        public string Isbn { get; set; }

        public DateTime PublishedDate { get; set; }

        [Range(1, 1000)]
        public int Stock { get; set; }

        [Range(0, 1000)]
        public int Available { get; set; }

        // 编辑书籍时,需要作者和出版社的 ID、书籍分类的 ID
        [Required]
        public int AuthorId { get; set; }

        [Required]
        public int PublisherId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "书籍至少需要一个分类")]
        public List<int> CategoryIds { get; set; } = [];
    }
}
