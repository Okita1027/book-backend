using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO_CRUD.Data
{
    public class IAuditableEntity
    {
        [Column(TypeName = "datetime(0)")]
        public DateTime UpdatedTime { get; set; }
    }
}
