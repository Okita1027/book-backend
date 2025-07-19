using DEMO_CRUD.Data;
using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await context.Authors.ToListAsync();
        }

        // GET: api/Authors/5
        // {id:int}作用：类型限制，若转换失败，则返回400 Bad Request
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
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
                return NotFound("此作者不存在！");
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

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            context.Authors.Remove(author);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return context.Authors.Any(e => e.Id == id);
        }
    }
}