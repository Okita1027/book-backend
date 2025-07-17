using DEMO_CRUD.Data;
using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
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
    }
}
