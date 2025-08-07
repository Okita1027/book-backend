using book_backend.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace book_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected ApplicationDbContext()
        {
        }

        // : base(options) 代表调用父类的构造器
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
        // 在操纵Book类时，系统会自动生成BooksCategories关联表的记录
        // 但为了更方便地操作中间表，还是在这里进行声明
        public DbSet<BookCategory> BookCategories { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 如果 DbContextOptions 还没有被配置 (即通过无参构造函数创建时)
            if (!optionsBuilder.IsConfigured)
            {
                // 直接在这里提供连接字符串和数据库提供程序
                string connectionString = "Server=localhost;Port=3306;Database=LibraryDb;Uid=root;Pwd=123456;";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                // 可选：启用敏感数据日志（仅用于开发/调试）
                // optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 批量设置所有实体在数据库中的表名称
            /*foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.Name);           // 实体类名
                entityType.SetTableName(entityType.ClrType.Name);   // 运行时类名
            }*/
            
            // 去除时间类型的默认精度
            /*modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(a => a.CreatedTime)
                      .HasColumnType("datetime(0)"); // MySQL 示例，表示不保留小数；SQL Server要写成datetime2(0)
            });*/

            // 开始设定模型
            // 1. 定义复合主键、手动指定表名称
            modelBuilder.Entity<BookCategory>()
                .ToTable("bookcategory")
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
            
            // 配置Author 和 Book 之间的一对多关系
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            
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
