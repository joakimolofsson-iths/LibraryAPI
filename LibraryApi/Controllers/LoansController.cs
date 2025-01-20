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
		public async Task<ActionResult<IEnumerable<LoanDisplayDTO>>> GetAllLoans()
		{
			var loans = await _context.Loans.ToListAsync();
			var loanDTOs = loans.Select(Mapper.ToLoanDisplayDTO).ToList();

			return Ok(loanDTOs);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<LoanDisplayDTO>> GetLoanById(int id)
		{
			var loan = await _context.Loans
				.Include(lo => lo.BookCopy)
				.ThenInclude(bc => bc.Book)
				.Include(lo => lo.Member)
				.FirstOrDefaultAsync(lo => lo.LoanId == id);

			if (loan == null)
			{
				return NotFound("Loan not found.");
			}

			var loanDisplayDTO = Mapper.ToLoanDisplayDTO(loan);

			return loanDisplayDTO;
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
				LoanDate = DateTime.Now,
			};

			bookCopy.OnLoan = true;

			_context.Loans.Add(loan);
			await _context.SaveChangesAsync();

			var fullLoan = await _context.Loans
				.Include(lo => lo.BookCopy)
				.ThenInclude(bc => bc.Book)
				.Include(lo => lo.Member)
				.FirstOrDefaultAsync(lo => lo.LoanId == loan.LoanId);

			if (fullLoan == null)
			{
				return NotFound();
			}

			var loanDisplayDTO = Mapper.ToLoanDisplayDTO(fullLoan);

			return CreatedAtAction(nameof(GetLoanById), new { id = loan.LoanId }, loanDisplayDTO);
		}

		[HttpPut("{id}/return")]
		public async Task<IActionResult> ReturnLoan(int id, LoanReturnDTO loanDTO)
		{
			var loan = await _context.Loans
				.Include(lo => lo.BookCopy)
				.FirstOrDefaultAsync(lo => lo.LoanId == id);

			if (loan == null)
			{
				return NotFound("Loan not found...");
			}

			loan.BookCopy.OnLoan = false;
			loan.ReturnDate = DateTime.Now;

			_context.Loans.Update(loan);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Book returned successfully...", returnDate = loan.ReturnDate });
		}
	}
}
