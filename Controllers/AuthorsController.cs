using System.Diagnostics;
using DEMO_CRUD.Data;
using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static DEMO_CRUD.Constants.IServiceConstants;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // 不加的结果===============【模型绑定】=====================
    public class AuthorsController(ApplicationDbContext context) : ControllerBase
    {
        // 下面是传统的构造函数写法，上面是C#特有的语法
        /*private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }*/

        // GET: api/Authors
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            var authors = await context.Authors.ToListAsync();
            return authors;
        }

        // GET: api/Authors/5
        // {id:int}作用：类型限制，若转换失败，则返回 400 Bad Request
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // PUT: api/Authors/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> PutAuthor(int id, EditAuthorDTO editAuthorDTO)
        {
            // 1.判断请求数据是否有效【符合注解的要求】
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2.查找现有实体
            var existingAuthor = await context.Authors.FindAsync(id);
            if (existingAuthor == null)
            {
                return NotFound(AUTHOR_NOT_FOUND);
            }

            // 3.更新现有实体的属性
            existingAuthor.Name = editAuthorDTO.Name;
            existingAuthor.Biography = editAuthorDTO.Biography;

            await context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Authors
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<Author>> PostAuthor(EditAuthorDTO editAuthorDTO)
        {
            var author = new Author
            {
                Name = editAuthorDTO.Name,
                Biography = editAuthorDTO.Biography
            };
            context.Authors.Add(author);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await context.Authors.FindAsync(id);
            if (author == null)
            {
                // return BadRequest(USER_NOT_FOUND);
                throw new ArgumentException();
            }

            context.Authors.Remove(author);
            await context.SaveChangesAsync();

            return NoContent();
        }
        
        // 批量删除
        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteAuthors([FromBody] List<int> ids)
        {
            var authors = await context.Authors.Where(a => ids.Contains(a.Id)).ToListAsync();
            if (authors.Count == 0)
            {
                return BadRequest(RECORD_NOT_FOUND);
            }

            context.Authors.RemoveRange(authors);
            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}