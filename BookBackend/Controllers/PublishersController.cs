using book_backend.Data;
using book_backend.Exceptions;
using book_backend.Models.DTO;
using book_backend.Models.Entity;
using book_backend.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static book_backend.Constants.IServiceConstants;
using static book_backend.Exceptions.IErrorCode;


namespace book_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController(ApplicationDbContext context) : ControllerBase
    {
        // GET: api/Publishers
        [HttpGet]
        // [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<List<Publisher>>> GetPublishers()
        {
            return await context.Publishers.ToListAsync();
        }

        // GET: api/Publishers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            var publisher = await context.Publishers.FindAsync(id);

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
            var existingPublisher = await context.Publishers.FindAsync(id);
            if (existingPublisher == null)
            {
                return BadRequest(RECORD_NOT_FOUND);
            }

            // 3.更新现有实体的属性
            existingPublisher.Name = editPublisherDTO.Name;
            await context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Publishers
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> PostPublisher(EditPublisherDTO editPublisherDTO)
        {
            // 判断该出版社是否已存在
            var existingPublisher = await context.Publishers.FirstOrDefaultAsync(p => p.Name == editPublisherDTO.Name);
            if (existingPublisher != null)
            {
                return BadRequest(PUBLISHER_ALREADY_EXISTS);
            }
            var publisher = new Publisher
            {
                Name = editPublisherDTO.Name
            };
            context.Publishers.Add(publisher);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetPublisher", new { id = publisher.Id }, publisher);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var publisher = await context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return BadRequest(PUBLISHER_NOT_FOUND);
            }

            context.Publishers.Remove(publisher);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // 批量删除出版社
        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeletePublishers([FromBody] List<int> ids)
        {
            var publishers = await context.Publishers.Where(p => ids.Contains(p.Id)).ToListAsync();
            if (publishers.Count == 0)
            {
                return BadRequest(RECORD_NOT_FOUND);
            }

            context.Publishers.RemoveRange(publishers);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}