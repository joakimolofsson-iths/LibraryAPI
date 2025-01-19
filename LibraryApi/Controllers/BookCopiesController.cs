using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;
using System.Net;

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
		public async Task<ActionResult<IEnumerable<BookCopyDisplayDTO>>> GetAllBookCopies()
		{
			var bookCopies = await _context.BookCopies
				.Include(bc => bc.Book)
				.ToListAsync();

			var bookCopiesDTOs = bookCopies.Select(Mapper.ToBookCopyDisplayDTO).ToList();

			return Ok(bookCopiesDTOs);
		}
	}
}
