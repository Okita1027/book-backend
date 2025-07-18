using DEMO_CRUD.Data;
using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        // 获取所有书籍
        [HttpGet]
        [AllowAnonymous]    // 允许匿名访问
        //public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            //// 使用 ToListAsync() 以异步方式获取所有书籍
            //List<Book> books = await _context.Books
            //    .Include(b => b.Author)     // 同时加载关联的 Author 实体
            //    .Include(b => b.Publisher)  // 同时加载关联的 Publisher 实体
            //    .ToListAsync();

            var books = await _context.Books
                .Include(b => b.Author)     // 同时加载关联的 Author 实体
                .Include(b => b.Publisher)  // 同时加载关联的 Publisher 实体
                .Select(book => new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    Stock = book.Stock,
                    Available = book.Available,
                    AuthorName = book.Author.Name,
                    PublisherName = book.Publisher.Name,
                })
                .ToListAsync();

            return Ok(books);   // 回传 HTTP 200 状态码和书籍列表
        }

        // GET: api/books/{id}
        // 根据 ID 获取特定书籍
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            // 使用 FindAsync() 以异步方式查找特定书籍
            var book = await _context.Books
                .Include(b => b.Author)     // 同时加载关联的 Author 实体
                .Include(b => b.Publisher)  // 同时加载关联的 Publisher 实体
                .Select(book => new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    Stock = book.Stock,
                    Available = book.Available,
                    AuthorName = book.Author.Name,
                    PublisherName = book.Publisher.Name,
                })
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound(); // 如果未找到书籍，回传 HTTP 404 状态码
            }
            return Ok(book); // 回传 HTTP 200 状态码和书籍详情
        }

        // 根据 书名/ISBN/作者名/出版社名/出版日期 模糊搜索书籍
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks(
            [FromQuery] string title = null,
            [FromQuery] string isbn = null,
            [FromQuery] string authorName = null,
            [FromQuery] string publisherName = null,
            [FromQuery] DateTime? publishedDateBegin = null,
            [FromQuery] DateTime? publishedDateEnd = null)
        {
            // 使用 LINQ 查询构建动态查询
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .AsQueryable();
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }
            if (!string.IsNullOrEmpty(isbn))
            {
                query = query.Where(b => b.Isbn.Contains(isbn));
            }
            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(b => b.Author.Name.Contains(authorName));
            }
            if (!string.IsNullOrEmpty(publisherName))
            {
                query = query.Where(b => b.Publisher.Name.Contains(publisherName));
            }
            if (publishedDateBegin.HasValue)
            {
                query = query.Where(b => b.PublishedDate >= publishedDateBegin.Value);
            }
            if (publishedDateEnd.HasValue)
            {
                query = query.Where(b => b.PublishedDate <= publishedDateEnd.Value);
            }
            var books = await query.Select(book => new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                Stock = book.Stock,
                Available = book.Available,
                AuthorName = book.Author.Name,
                PublisherName = book.Publisher.Name,
                PublishedDate = book.PublishedDate.ToString("yyyy-MM-dd")
            }).ToListAsync();
            return Ok(books);
        }

        // 借书
        [HttpPost("borrow")]
        [Authorize]
        public async Task<ActionResult<string>> BorrowBook(int id, string userName)
        {
            // 判断是否存在该id的书籍，并判断该书籍是否有可借数量
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("书籍未找到！");
            }
            if (book.Available <= 0)
            {
                return BadRequest("该书籍已无可借数量！");
            }
            // 判断用户是否存在
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userName);
            if (user == null)
            {
                return NotFound("用户未找到！");
            }
            // 创建借书记录
            var loan = new Loan
            {
                BookId = book.Id,
                UserId = user.Id,
                // 借书日期是今天，预计还书日期是1个月后
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddMonths(1),
                ReturnDate = null // 初始时未归还
            };
            // 将借书记录添加到数据库上下文中
            _context.Loans.Add(loan);
            // 更新书籍的可用数量
            book.Available -= 1; // 借出一本书，减少可用数量
            // 尝试保存更改到数据库
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // 处理数据库更新异常
                return BadRequest($"借书失败: {ex.Message}");
            }
            return Ok("借书成功！");
        }

        // 还书
        [HttpPost("return")]
        public async Task<ActionResult<string>> ReturnBook(int id, string userName)
        {
            // 查找借书记录
            var loan = await _context.Loans
                .Include(l => l.Book) // 同时加载关联的 Book 实体
                .FirstOrDefaultAsync(l => l.BookId == id && l.User.Name == userName && l.ReturnDate == null);
            if (loan == null)
            {
                return NotFound("借书记录未找到或已归还！");
            }
            // 更新还书日期
            loan.ReturnDate = DateTime.Now;
            // 更新书籍的可用数量
            loan.Book.Available += 1; // 归还一本书，增加可用数量
            // 保存更改到数据库
            await _context.SaveChangesAsync();
            return Ok("还书成功！");
        }

        // POST: api/books
        // 添加新的书籍
        [HttpPost]
        public async Task<ActionResult<string>> AddBook([FromBody] EditBookDTO bookDTO)
        {
            // 检查传入的 AuthorID 和 PublisherID 是否存在
            var author = await _context.Authors.FindAsync(bookDTO.AuthorId);
            var publisher = await _context.Publishers.FindAsync(bookDTO.PublisherId);
            if (author == null)
            {
                return BadRequest("作者不存在！");
            }
            if (publisher == null)
            {
                return BadRequest("出版社不存在！");
            }
            // 检查书籍类别是否存在
            if (bookDTO.CategoryIds == null || bookDTO.CategoryIds.Count == 0)
            {
                return BadRequest("书籍至少需要一个分类！");
            }
            // 验证传入的所有分类ID是否存在
            var existingCategories = await _context.Categories
                .Where(c => bookDTO.CategoryIds.Contains(c.Id))
                .ToListAsync();
            if (existingCategories.Count != bookDTO.CategoryIds.Count)
            {
                // 找出不存在的类别ID
                var nonExistentCategoryIds = bookDTO.CategoryIds
                                                    .Except(existingCategories.Select(c => c.Id))
                                                    .ToList();
                return BadRequest($"部分或全部书籍类别不存在：{string.Join(", ", nonExistentCategoryIds)}");
            }


            // DTO映射
            var newBook = new Book
            {
                Title = bookDTO.Title,
                Isbn = bookDTO.Isbn,
                PublishedDate = bookDTO.PublishedDate,
                Stock = bookDTO.Stock,
                Available = bookDTO.Stock, // 初始可用数量等于库存数量
                AuthorId = bookDTO.AuthorId,
                PublisherId = bookDTO.PublisherId
            };

            // 将书籍的新实体添加到数据库上下文中
            _context.Books.Add(newBook);

            // 添加书籍类别和书籍的关联(处理联结表BookCategory)
            // 在保存newBook后，它会获得数据库生成的主键ID
            // 但在SaveChanges()之前，就可以建立BookCategory的关系
            foreach (var category in existingCategories)
            {
                newBook.BookCategories.Add(new BookCategory
                {
                    Category = category,
                });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // 处理数据库更新异常，例如约束冲突等
                // 打印详细错误信息有助于调试
                Console.WriteLine($"Error saving book and categories: {ex.InnerException?.Message ?? ex.Message}");
                return StatusCode(500, "创建书籍失败，请稍后再试。");
            }

            return StatusCode(201, "书籍创建成功！");
        }

        // PUT: api/books/{id}
        // 修改图书
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateBook(int id, [FromBody] EditBookDTO bookDTO)
        {
            // 查找要更新的书籍
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("书籍未找到！");
            }
            // 检查传入的 AuthorID 和 PublisherID 是否存在
            var author = await _context.Authors.FindAsync(bookDTO.AuthorId);
            var publisher = await _context.Publishers.FindAsync(bookDTO.PublisherId);
            if (author == null || publisher == null)
            {
                return BadRequest("作者/出版社不存在！");
            }

            if (bookDTO.Available > bookDTO.Stock)
            {
                return BadRequest("可用库存必须小于总库存");
            }

            // 更新书籍属性
            book.Title = bookDTO.Title;
            book.Isbn = bookDTO.Isbn;
            book.PublishedDate = bookDTO.PublishedDate;
            book.Stock = bookDTO.Stock;
            book.Available = bookDTO.Available; // 更新可用数量
            book.AuthorId = bookDTO.AuthorId;
            book.PublisherId = bookDTO.PublisherId;
            // 保存更改到数据库
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/books/{id}
        // 删除图书
        [HttpDelete("id")]
        public async Task<ActionResult<string>> DeleteBook(int id)
        {
            // 查找要删除的书籍
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("书籍未找到！");
            }
            // 从数据库上下文中移除书籍
            _context.Books.Remove(book);
            // 保存更改到数据库
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
