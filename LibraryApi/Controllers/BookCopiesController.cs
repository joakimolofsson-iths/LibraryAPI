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
	public class BookCopiesController : ControllerBase
	{
		private readonly LibraryDbContext _context;
		public BookCopiesController(LibraryDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<BookCopy>>> GetAllBookCopies()
		{
			var bookCopies = await _context.BookCopies.ToListAsync();

			if (bookCopies == null || !bookCopies.Any())
			{
				return NotFound();
			}

			return Ok(bookCopies);
		}
	}
}
