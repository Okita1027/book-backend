using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO_CRUD.Data
{
    public class AuditableEntity
    {
        [Column(TypeName = "datetime(0)")]
        public DateTime UpdatedTime { get; set; }
    }
}
