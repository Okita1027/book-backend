using DEMO_CRUD.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace DEMO_CRUD.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // 为每个需要映射到资料库表格的实体建立一个 DbSet
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Fine> Fines { get; set; }

        // Fluent API 设定
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 去除时间类型的默认精度（Fluent API方式）
            //modelBuilder.Entity<Author>(entity =>
            //{
            //    entity.Property(a => a.CreatedTime)
            //          .HasColumnType("datetime(0)"); // MySQL 示例，表示不保留小数
            //});

            // 开始设定模型
            // 1. 定义复合主键
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });

            // 2. 设定从 BookCategory 到 Book 的关系
            // 一个BookCategory 只能对应一个 Book
            // 一个Book 可以对应多个 BookCategory 记录
            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookCategories)
                .HasForeignKey(bc => bc.BookId);

            // 3. 设定从 BookCategory 到 Category 的关系
            // 一个BookCategory 只能对应一个 Category
            // 一个Category 可以对应多个 BookCategory 记录
            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(bc => bc.CategoryId);
        }
        
        //重写 SaveChanges 和 SaveChangesAsync 方法，用于自动更新 UpdatedTime
        public override int SaveChanges()
        {
            ApplyAuditInformation(); // 调用私有方法来更新 UpdatedTime
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation(); // 调用私有方法来更新 UpdatedTime
            return await base.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// 私有辅助方法，用于遍历被修改的实体并更新 UpdatedTime。
        /// </summary>
        private void ApplyAuditInformation()
        {
            // 获取所有处于 Added (新增) 或 Modified (修改) 状态的实体条目
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditableEntity auditableEntity) // 检查实体是否实现了 AuditableEntity
                {
                    if (entry.State == EntityState.Modified)
                    {
                        // 如果实体被修改，更新 UpdatedTime
                        auditableEntity.UpdatedTime = DateTime.Now;

                        // 可选：如果想确保 CreatedTime 不被修改，可以在这里处理
                        // entry.Property(nameof(AuditableEntity.CreatedTime)).IsModified = false;
                    }
                    // 也可以在这里处理 EntityState.Added 状态的实体，如果它们的 CreatedTime 没有在构造函数中设置
                    // else if (entry.State == EntityState.Added)
                    // {
                    //     auditableEntity.CreatedTime = DateTime.Now;
                    //     auditableEntity.UpdatedTime = DateTime.Now;
                    // }
                }
            }
        }

    }
}
