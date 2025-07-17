using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.DTO
{
    public class EditAuthorDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Biography { get; set; } = string.Empty;
    }
}
