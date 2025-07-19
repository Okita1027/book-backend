using DEMO_CRUD.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO_CRUD.Models.Entity
{
    public class Book : AuditableEntity
    {
        public int Id { get; set; } // 主键

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } // 书名

        [Required]
        [MaxLength(13)]
        // ISBN-13 标准长度为 13
        public string Isbn { get; set; } // 国际标准书号，唯一标识

        public DateTime PublishedDate { get; set; } //出版日期

        [Required]
        public int Stock { get; set; } // 总库存数量

        [Required]
        public int Available { get; set; } // 当前可用数量

        // --- 导航属性 (Navigation Properties) ---

        // 外键
        public int AuthorId { get; set; }
        public int PublisherId { get; set; }

        // 一个 Book 属于一个 Author
        public Author Author { get; set; }

        // 一个 Book 属于一个 Publisher
        public Publisher Publisher { get; set; }

        // 一个 Book 可以属于多个 Category (多对多)
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

        // 一个 Book 可以有多个借阅记录
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();


        // 通用属性：创建时间、更新时间
        [Column(TypeName="datetime(0)")]
        public DateTime CreatedTime { get; init; } = DateTime.Now;
        //public DateTime UpdatedTime { get; set; } = DateTime.Now;

    }
}
