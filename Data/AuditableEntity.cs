using System.ComponentModel.DataAnnotations.Schema;

namespace book_backend.Data
{
    public class AuditableEntity
    {
        [Column(TypeName = "datetime(0)")]
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
