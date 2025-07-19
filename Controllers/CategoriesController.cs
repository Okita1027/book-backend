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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
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

            return category;
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
                return NotFound("此分类不存在！");
            }

            // 3.更新现有实体的属性
            existingCategory.Name = editCategoryDTO.Name;

            // 4.尝试保存更改
            await _context.SaveChangesAsync();
            return Ok("修改成功");
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<Category>> PostCategory(EditCategoryDTO editCategoryDTO)
        {
            var category = new Category
            {
                Name = editCategoryDTO.Name
            };
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
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}