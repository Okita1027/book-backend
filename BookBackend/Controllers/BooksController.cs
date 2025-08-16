using book_backend.Models.DTO;
using book_backend.Models.Entity;
using book_backend.Services;
using book_backend.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace book_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]  // 为该类所有方法添加认证
    public class BooksController(IBooksService booksService) : ControllerBase
    {
        /// <summary>
        /// 获取所有书籍 TODO:待优化
        /// </summary>
        /// <returns>原生类不加工</returns>
        [HttpGet("/api/RawBooks")]
        public async Task<ActionResult<List<Book>>> GetRawBooks()
        {
            return Ok(await booksService.GetAllRawBooksAsync());
        }

        /// <summary>
        /// 获取所有的书籍
        /// </summary>
        /// <returns>处理过的用于展示的图书集</returns>
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        // [AllowAnonymous] // 忽略类上写的的[Authorize]注解，允许匿名访问
        public async Task<ActionResult<List<BookVO>>> GetBooks()
        {
            var books = await booksService.GetAllBooksAsync();
            return Ok(books);
        }

        /// <summary>
        /// 根据ID获取书籍
        /// </summary>
        /// <param name="id">图书ID</param>
        /// <returns>处理过后的用于展示的图书</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookVO>> GetBook(int id)
        {
            var book = await booksService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound("未找到该书籍");
            }

            return Ok(book);
        }

        /// <summary>
        /// 图书搜索
        /// </summary>
        /// <param name="title"></param>
        /// <param name="isbn"></param>
        /// <param name="categoryName"></param>
        /// <param name="authorName"></param>
        /// <param name="publisherName"></param>
        /// <param name="publishedDateBegin"></param>
        /// <param name="publishedDateEnd"></param>
        /// <returns>处理过后的用于展示的图书</returns>
        [HttpGet("search")]
        [Obsolete("请使用下面的分页查询")]
        public async Task<ActionResult<List<BookVO>>> SearchBooks(
            [FromQuery] string? title = null,
            [FromQuery] string? isbn = null,
            [FromQuery] string? categoryName = null,
            [FromQuery] string? authorName = null,
            [FromQuery] string? publisherName = null,
            [FromQuery] DateTime? publishedDateBegin = null,
            [FromQuery] DateTime? publishedDateEnd = null)
        {
            var books = await booksService.SearchBooksAsync(title, isbn, categoryName, authorName, publisherName,
                publishedDateBegin,
                publishedDateEnd);
            return Ok(books);
        }

        [HttpGet("searchPaginated")]
        public async Task<Pagination<BookVO>> SearchBooksPaginated(
            [FromQuery] PaginationRequest paginationRequest,
            [FromQuery] string? title = null,
            [FromQuery] string? isbn = null,
            [FromQuery] string? categoryName = null,
            [FromQuery] string? authorName = null,
            [FromQuery] string? publisherName = null,
            [FromQuery] DateTime? publishedDateBegin = null,
            [FromQuery] DateTime? publishedDateEnd = null)
        {
            var books = await booksService.SearchBooksPaginatedAsync(
                paginationRequest, title, isbn, categoryName, authorName, publisherName, 
                publishedDateBegin, publishedDateEnd);
            return books;
        }

        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="id">图书的ID</param>
        /// <param name="username">用户的名称</param>
        /// <returns>借书结果提示信息</returns>
        [HttpPost("loan")]
        [Authorize]
        [Obsolete("请使用LoansController中的借书方法")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> LoanBook(int id, string username)
        {
            var result = await booksService.LoanBookAsync(id, username);
            return Ok(result);
        }

        /// <summary>
        /// 还书
        /// </summary>
        /// <param name="id">图书的ID</param>
        /// <param name="username">用户的名称</param>
        /// <returns>还书提示信息</returns>
        [HttpPost("return")]
        [Authorize]
        [Obsolete("请使用LoansController中的还书方法")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<string>> ReturnBook(int id, string username)
        {
            var result = await booksService.ReturnBookAsync(id, username);
            return Ok(result);
        }

        /// <summary>
        /// 添加新的书籍
        /// </summary>
        /// <param name="bookDTO">编辑时所需要的属性构成的书籍类</param>
        /// <returns>新增图书的结果</returns>
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook([FromBody] EditBookDTO bookDTO)
        {
            try
            {
                var generatedId = await booksService.AddBookAsync(bookDTO);
                return CreatedAtAction("GetBook", new { id = generatedId }, bookDTO);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// 更新图书的信息
        /// </summary>
        /// <param name="id">图书ID</param>
        /// <param name="bookDTO">编辑时所需要的属性构成的书籍类</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] EditBookDTO bookDTO)
        {
            try
            {
                await booksService.UpdateBookAsync(id, bookDTO);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 删除书籍
        /// </summary>
        /// <param name="id">书籍ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
        {
            try
            {
                await booksService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 批量删除书籍
        /// </summary>
        /// <param name="ids">ID的集合</param>
        /// <returns>删除结果</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteBooks([FromBody] List<int> ids)
        {
            try
            {
                await booksService.DeleteBooksAsync(ids);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}