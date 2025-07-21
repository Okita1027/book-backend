using DEMO_CRUD.Data;
using DEMO_CRUD.Models.Entity;
using DEMO_CRUD.Models.VO;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static DEMO_CRUD.Constants.IServiceConstants;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FinesController(ApplicationDbContext context) : ControllerBase
    {
        // 查询所有罚款记录
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FineVO>>> GetFines()
        {
            List<Fine> fines = await context.Fines.Include(f => f.Loan).Include(f => f.User).ToListAsync();
            List<FineVO> result = fines.Adapt<List<FineVO>>();
            return result;
        }

        //根据ID查询某个罚款记录
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FineVO>> GetFine([FromRoute] int id)
        {
            var fine = await context.Fines.FindAsync(id);
            if (fine == null)
            {
                return NotFound();
            }
            FineVO fineVO = fine.Adapt<FineVO>();
            return Ok(fineVO);
        }


        /// <summary>
        /// 支付罚款
        /// </summary>
        /// <param name="loanId">借书的ID</param>
        /// <param name="userId">用户的ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Fine(int loanId, int userId)
        {
            // 根据loanId和userId查询欠款记录
            var fine = await context.Fines.FirstOrDefaultAsync(fine => fine.LoanId == loanId && fine.UserId == userId);
            if (fine == null)
            {
                return BadRequest(RECORD_NOT_FOUND);
            }

            fine.PaidDate = DateTime.Now;
            await context.SaveChangesAsync();
            return NoContent();
        }

        // 删除罚款记录
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteFine(int id)
        {
            var fine = await context.Fines.FindAsync(id);
            if (fine == null)
            {
                return BadRequest(RECORD_NOT_FOUND);
            }

            context.Fines.Remove(fine);
            await context.SaveChangesAsync();
            return NoContent();
        }


        // 批量删除罚款记录
        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteFines([FromBody] List<int> ids)
        {
            var fines = await context.Fines.Where(f => ids.Contains(f.Id)).ToListAsync();
            if (fines.Count == 0)
            {
                return BadRequest(RECORD_NOT_FOUND);
            }

            context.Fines.RemoveRange(fines);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}