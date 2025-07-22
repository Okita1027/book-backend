using DEMO_CRUD.Data;
using DEMO_CRUD.Exceptions;
using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
using DEMO_CRUD.Models.VO;
using DEMO_CRUD.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static DEMO_CRUD.Constants.IServiceConstants;

namespace DEMO_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController(ApplicationDbContext context, ILoansService loansService) : ControllerBase
    {
        // 获取所有借书记录
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<IEnumerable<LoanVO>>> GetLoans()
        {
            List<LoanVO> loanVos = await context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ProjectToType<LoanVO>()
                .ToListAsync();
            return loanVos;
        }

        // 根据ID获取借阅记录
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LoanVO>> GetLoan(int id)
        {
            var loan = await context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            LoanVO loanVo = loan.Adapt<LoanVO>();
            return loanVo;
        }

        // 根据用户名称获取借阅记录
        [HttpGet]
        [Route("/api/LoansByName")]
        public async Task<ActionResult<IEnumerable<LoanVO>>> GetLoansByName([FromQuery] string username)
        {
            List<LoanVO> loanVos = await context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.User.Name == username)
                .ProjectToType<LoanVO>()
                .ToListAsync();
            return loanVos;
        }

        // 借书
        [HttpPost]
        [Authorize]
        [Route("/api/Loan")]
        public async Task<IActionResult> LoanBook([FromBody] EditLoanDTO editLoanDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await loansService.LoanBookAsync(editLoanDto);
                return Ok(OPERATION_SUCCESS);
            }
            catch (Exception e) when (e is ArgumentException or BusinessException)
            {
                return BadRequest(e.Message);
            }
        }
        
        // 还书
        [HttpPost]
        [Authorize]
        [Route("/api/Return")]
        public async Task<IActionResult> ReturnBook([FromQuery] int bookId, [FromQuery] int userId)
        {
            try
            {
                await loansService.ReturnBookAsync(bookId, userId);
                return Ok(OPERATION_SUCCESS);
            }
            catch (Exception e) when (e is ArgumentException or BusinessException)
            {
                return BadRequest(e.Message);
            }
        }

        // 修改应还日期
        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateDueDate([FromRoute]int id, [FromQuery]DateTime dueDate)
        {
            var loan = await context.Loans.FindAsync(id);
            if (loan == null)
            {
                return BadRequest(RECORD_NOT_FOUND);
            }
            loan.DueDate = dueDate;
            await context.SaveChangesAsync();
            return Ok(OPERATION_SUCCESS);
        }

        // 删除借阅记录
        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await context.Loans.FindAsync(id);
            if (loan == null)
            {
                return BadRequest(RECORD_NOT_FOUND);
            }

            context.Loans.Remove(loan);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}