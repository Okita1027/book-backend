using System.ComponentModel.DataAnnotations;

namespace book_backend.Models.DTO
{
    public class EditAuthorDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Biography { get; set; } = string.Empty;
    }
}
