using DEMO_CRUD.Data;
using DEMO_CRUD.Models.Entity;
using Masuit.Tools.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinesController(ApplicationDbContext context) : ControllerBase
    {
        [HttpPost("fine")]
        [Authorize]
        public async Task<IActionResult> Fine(int loanId, int userId)
        {
            // 根据loanId和userId查询欠款记录
            var fine = await context.Fines.FirstOrDefaultAsync(fine => fine.LoanId == loanId && fine.UserId == userId);
            if (fine == null)
            {
                return BadRequest("未找到该罚款记录");
            }

            fine.PaidDate = DateTime.Now;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}