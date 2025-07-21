using System.ComponentModel.DataAnnotations;

namespace DEMO_CRUD.Models.DTO;

public class EditLoanDTO
{
    [Required]
    public int BookId { get; set; }
    [Required]
    public int UserId { get; set; }
    public DateTime DueDate { get; set; } = DateTime.MinValue;
}