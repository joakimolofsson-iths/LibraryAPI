using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoansController : ControllerBase
	{
		private readonly LibraryDbContext _context;
		public LoansController(LibraryDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Loan>>> GetAllLoans()
		{
			var loans = await _context.Loans.ToListAsync();

			if (loans == null || !loans.Any())
			{
				return NotFound();
			}

			return Ok(loans);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Loan>> GetLoan(int id)
		{
			var loan = await _context.Loans
				.Include(bl => bl.BookCopy)
				.ThenInclude(bc => bc.Book)
				.Include(bl => bl.Member)
				.FirstOrDefaultAsync(bl => bl.LoanId == id);

			if (loan == null)
			{
				return NotFound("Loan not found.");
			}

			return loan;
		}

		[HttpPost]
		public async Task<ActionResult<Loan>> LoanBook(LoanCreateDTO loanDTO)
		{
			var bookCopy = await _context.BookCopies
				.FirstOrDefaultAsync(bc => bc.BookCopyId == loanDTO.BookCopyId && !bc.OnLoan);

			if (bookCopy == null)
			{
				return NotFound("The book copy is on loan or does not exist...");
			}

			var member = await _context.Members.FindAsync(loanDTO.MemberId);
			if (member == null)
			{
				return NotFound("Member not found...");
			}

			var loan = new Loan
			{
				BookCopyId = bookCopy.BookCopyId,
				MemberId = member.MemberId,
				LoanDate = DateTime.UtcNow,
			};

			bookCopy.OnLoan = true;

			_context.Loans.Add(loan);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetLoan), new { id = loan.LoanId }, loan);
		}

		[HttpPut("{id}/return")]
		public async Task<IActionResult> ReturnLoan(int id, LoanReturnDTO loanDTO)
		{
			var loan = await _context.Loans
				.Include(bl => bl.BookCopy)
				.FirstOrDefaultAsync(bl => bl.LoanId == id);

			if (loan == null)
			{
				return NotFound("Loan not found...");
			}

			loan.BookCopy.OnLoan = false;

			loan.ReturnDate = loanDTO.ReturnDate;

			_context.Loans.Update(loan);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
