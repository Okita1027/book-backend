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
    public class PublishersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublishersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishers()
        {
            return await _context.Publishers.ToListAsync();
        }

        // GET: api/Publishers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // PUT: api/Publishers/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> PutPublisher(int id, EditPublisherDTO editPublisherDTO)
        {
            // 1.判断请求数据是否有效【符合注解的要求】
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2.查找现有实体
            var existingPublisher = await _context.Publishers.FindAsync(id);
            if (existingPublisher == null)
            {
                return NotFound("此出版社不存在！");
            }

            // 3.更新现有实体的属性
            existingPublisher.Name = editPublisherDTO.Name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Publishers
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<Publisher>> PostPublisher(EditPublisherDTO editPublisherDTO)
        {
            var publisher = new Publisher
            {
                Name = editPublisherDTO.Name
            };
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublisher", new { id = publisher.Id }, publisher);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }
    }
}