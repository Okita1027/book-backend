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
    [Authorize] // 鉴权注解，必须通过JWT认证才能访问该类中的方法
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


        // POST: api/books
        // 添加新的书籍
        [HttpPost]
        public async Task<ActionResult<string>> AddBook([FromBody]EditBookDTO book)
        {
            // 检查传入的 AuthorID 和 PublisherID 是否存在
            var author = await _context.Authors.FindAsync(book.AuthorId);
            var publisher = await _context.Publishers.FindAsync(book.PublisherId);
            if (author == null || publisher == null)
            {
                return BadRequest("作者/出版社不存在！");
            }

            // DTO映射
            var newBook = new Book
            {
                Title = book.Title,
                Isbn = book.Isbn,
                PublishedDate = book.PublishedDate,
                Stock = book.Stock,
                Available = book.Stock, // 初始可用数量等于库存数量
                AuthorId = book.AuthorId,
                PublisherId = book.PublisherId
            };

            // 将新实体添加到数据库上下文中
            _context.Books.Add(newBook);
            // 保存更改到数据库
            await _context.SaveChangesAsync();

            return Ok("创建成功！");
        }

        // PUT: api/books/{id}
        // 修改图书
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateBook(int id, [FromBody]EditBookDTO bookDTO)
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
