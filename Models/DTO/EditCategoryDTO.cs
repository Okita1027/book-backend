using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.DTO
{
    public class EditCategoryDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
