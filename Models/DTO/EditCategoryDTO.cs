using System.ComponentModel.DataAnnotations;

namespace book_backend.Models.DTO
{
    public class EditCategoryDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
