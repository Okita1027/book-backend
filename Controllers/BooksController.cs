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
    // [Authorize]  // 为所有方法添加鉴权
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有的书籍
        /// </summary>
        /// <returns>处理过的用于展示的数据集</returns>
        [HttpGet]
        // [AllowAnonymous]    // 忽略类上的[Authorize]注解，允许匿名访问
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            List<BookDTO> books = await _context.Books
                .Include(b => b.Author) // 同时加载关联的 Author 实体
                .Include(b => b.Publisher) // 同时加载关联的 Publisher 实体
                .Select(book => new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    Stock = book.Stock,
                    Available = book.Available,
                    AuthorName = book.Author.Name,
                    PublisherName = book.Publisher.Name,
                    PublishedDate = book.PublishedDate,
                    CategoryNames = book.BookCategories.Select(bc => bc.Category.Name).ToList()
                })
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/books/{id}
        // 根据 ID 获取特定书籍
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author) // 同时加载关联的 Author 实体
                .Include(b => b.Publisher) // 同时加载关联的 Publisher 实体
                .Select(book => new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    Stock = book.Stock,
                    Available = book.Available,
                    AuthorName = book.Author.Name,
                    PublisherName = book.Publisher.Name,
                    PublishedDate = book.PublishedDate,
                    CategoryNames = book.BookCategories.Select(bc => bc.Category.Name).ToList()
                })
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound("未找到该书籍");
            }

            return Ok(book);
        }

        /// <summary>
        /// 根据 书名/ISBN/作者名/出版社名/出版日期 模糊搜索书籍
        /// </summary>
        /// <param name="title"></param>
        /// <param name="isbn"></param>
        /// <param name="authorName"></param>
        /// <param name="publisherName"></param>
        /// <param name="publishedDateBegin"></param>
        /// <param name="publishedDateEnd"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks(
            [FromQuery] string title = null,
            [FromQuery] string isbn = null,
            [FromQuery] string authorName = null,
            [FromQuery] string publisherName = null,
            [FromQuery] DateTime? publishedDateBegin = null,
            [FromQuery] DateTime? publishedDateEnd = null)
        {
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

            List<BookDTO> books = await query.Select(book => new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                Stock = book.Stock,
                Available = book.Available,
                AuthorName = book.Author.Name,
                PublisherName = book.Publisher.Name,
                PublishedDate = book.PublishedDate,
                CategoryNames = book.BookCategories.Select(bc => bc.Category.Name).ToList()
            }).ToListAsync();
            return Ok(books);
        }

        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="id">书籍的ID</param>
        /// <param name="username">用户的名称</param>
        /// <returns>借阅结果</returns>
        [HttpPost("loan")]
        [Authorize]
        public async Task<ActionResult<string>> LoanBook(int id, string username)
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
            if (user == null)
            {
                return NotFound("未找到该用户！");
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
            _context.Loans.Add(loan);
            book.Available -= 1; // 借出一本书，减少可用数量
            await _context.SaveChangesAsync();
            return CreatedAtAction("LoanBook", new { id = book.Id }, book);
        }

        /// <summary>
        /// 还书
        /// </summary>
        /// <param name="id">书籍的ID</param>
        /// <param name="username">用户的名称</param>
        /// <returns>还书结果</returns>
        [HttpPost("return")]
        [Authorize]
        public async Task<ActionResult<string>> ReturnBook(int id, string username)
        {
            // 查找借书记录
            var loan = await _context.Loans
                .Include(l => l.Book) // 同时加载关联的 Book
                .FirstOrDefaultAsync(l => l.BookId == id && l.User.Name == username && l.ReturnDate == null);
            if (loan == null)
            {
                return NotFound("借书记录未找到或已归还！");
            }

            // 更新还书日期
            loan.ReturnDate = DateTime.Now;
            // 若超期还书，则生成罚款单
            if (loan.DueDate < loan.ReturnDate)
            {
                TimeSpan span = loan.ReturnDate.Value - loan.DueDate;
                // span.Days VS span.TotalDays : 前者是整数，后者带小数
                int spanDays = span.Days;
                var fine = new Fine
                {
                    Amount = 0.5m * spanDays,
                    Reason = "超期还书",
                    LoanId = loan.Id,
                    UserId = loan.UserId
                };
                _context.Fines.Add(fine);
            }

            // 更新书籍的可用数量
            loan.Book.Available += 1;

            await _context.SaveChangesAsync();
            return Ok("还书成功！");
        }

        /// <summary>
        /// 添加新的书籍
        /// </summary>
        /// <param name="bookDTO">书籍类的部分字段</param>
        /// <returns>添加结果</returns>
        [HttpPost("add")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> AddBook([FromBody] EditBookDTO bookDTO)
        {
            // 检查传入的 AuthorID 和 PublisherID 是否存在
            var author = await _context.Authors.FindAsync(bookDTO.AuthorId);
            if (author == null)
            {
                return BadRequest("作者不存在！");
            }

            var publisher = await _context.Publishers.FindAsync(bookDTO.PublisherId);
            if (publisher == null)
            {
                return BadRequest("出版社不存在！");
            }

            // 检查ISBN是否已存在
            if (await _context.Books.FirstOrDefaultAsync(b => b.Isbn == bookDTO.Isbn) != null)
            {
                return BadRequest("ISBN已存在！");
            }

            // 检查书籍类别是否存在
            if (bookDTO.CategoryIds.Count == 0)
            {
                return BadRequest("书籍至少需要一个分类！");
            }

            // 验证传入的所有分类ID是否存在
            List<Category> existingCategories = await _context.Categories
                .Where(c => bookDTO.CategoryIds.Contains(c.Id))
                .ToListAsync();
            if (existingCategories.Count != bookDTO.CategoryIds.Count)
            {
                // 找出不存在的类别ID
                List<int> nonExistentCategoryIds = bookDTO.CategoryIds
                    .Except(existingCategories.Select(c => c.Id))
                    .ToList();
                return BadRequest($"部分或全部书籍类别不存在：{string.Join(", ", nonExistentCategoryIds)}");
            }

            // DTO映射
            Book book = new Book
            {
                Title = bookDTO.Title,
                Isbn = bookDTO.Isbn,
                PublishedDate = bookDTO.PublishedDate,
                Stock = bookDTO.Stock,
                Available = bookDTO.Stock, // 初始可用数量等于库存数量
                AuthorId = bookDTO.AuthorId,
                PublisherId = bookDTO.PublisherId
            };

            _context.Books.Add(book);

            // 添加书籍类别和书籍的关联(处理联结表BookCategory)
            // 保存book后会获得数据库生成的ID，在SaveChanges()之前，就可以建立BookCategory的关系
            foreach (Category category in existingCategories)
            {
                book.BookCategories.Add(new BookCategory
                {
                    Category = category,
                });
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction("GetBook", new { id = book.Id }, bookDTO);
        }

        /// <summary>
        /// 修改图书
        /// </summary>
        /// <param name="id">书籍的ID</param>
        /// <param name="bookDTO">更新书籍所需要的字段</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] EditBookDTO bookDTO)
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
                return BadRequest("可用库存必须小于总库存！");
            }

            // 更新书籍属性
            book.Title = bookDTO.Title;
            book.Isbn = bookDTO.Isbn;
            book.PublishedDate = bookDTO.PublishedDate;
            book.Stock = bookDTO.Stock;
            book.Available = bookDTO.Available;
            book.AuthorId = bookDTO.AuthorId;
            book.PublisherId = bookDTO.PublisherId;
            // 重置图书的分类
            book.BookCategories.Clear();
            foreach (var categoryId in bookDTO.CategoryIds)
            {
                var bookCategory = new BookCategory
                {
                    BookId = book.Id,
                    CategoryId = categoryId
                };
                book.BookCategories.Add(bookCategory);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/books/{id}
        // 删除图书
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return BadRequest("不存在该书籍！");
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}