using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEMO_CRUD.Data;
using DEMO_CRUD.Models.Entity;
using DEMO_CRUD.Models.DTO;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await _context.Authors.ToListAsync();
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // PUT: api/Authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, EditAuthorDTO editAuthorDTO)
        {
            // 1.判断请求数据是否有效【符合注解的要求】
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // 2.查找现有实体
            var existingAuthor = await _context.Authors.FindAsync(id);
            if (existingAuthor == null)
            {
                return NotFound("此作者不存在！");
            }
            // 3.更新现有实体的属性
            existingAuthor.Name = editAuthorDTO.Name;
            existingAuthor.Biography = editAuthorDTO.Biography;

            // 4.尝试保存更改
            try
            {
                await _context.SaveChangesAsync();
            }
            // 5.处理并发异常
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    // 如果在尝试保存时发现实体不存在，说明已经被另一个操作删除了
                    return NotFound("此作者不存在！");
                }
                else
                {
                    throw; // 重新抛出异常以供全局异常处理器处理
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(EditAuthorDTO editAuthorDTO)
        {
            var author = new Author
            {
                Name = editAuthorDTO.Name,
                Biography = editAuthorDTO.Biography
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
