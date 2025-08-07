using book_backend.Data;
using book_backend.Models.DTO;
using book_backend.Models.Entity;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static book_backend.Constants.IServiceConstants;

namespace book_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> PutCategory(int id, EditCategoryDTO editCategoryDTO)
        {
            // 1.判断请求数据是否有效【符合注解的要求】
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2.查找现有实体
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound(CATEGORY_NOT_FOUND);
            }

            // 3.更新现有实体的属性
            existingCategory.Name = editCategoryDTO.Name;

            // 4.保存更改
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> PostCategory(EditCategoryDTO editCategoryDTO)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == editCategoryDTO.Name);
            if (existingCategory != null)
            {
                return BadRequest(CATEGORY_ALREADY_EXISTS);
            }
            var category = editCategoryDTO.Adapt<Category>();
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return BadRequest(CATEGORY_NOT_FOUND);
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // 批量删除
        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteCategories([FromBody] List<int> ids)
        {
            var categories = await _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync();
            if (categories.Count == 0)
            {
                return BadRequest(CATEGORY_NOT_FOUND);
            }

            _context.Categories.RemoveRange(categories);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}