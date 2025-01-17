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
	public class BooksController : ControllerBase
	{
		private readonly LibraryDbContext _context;
		public BooksController(LibraryDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<BookDisplayDTO>>> GetAllBooks()
		{
			var books = await _context.Books
				.Include(b => b.BookAuthors)
				.ThenInclude(ba => ba.Author)
				.Select(b => new BookDisplayDTO
				{
					BookId = b.BookId,
					Title = b.Title,
					ISBN = b.ISBN,
					YearPublished = b.YearPublished,
					Rating = b.Rating,
					Authors = b.BookAuthors.Select(ba => new AuthorDisplayDTO
					{
						AuthorId = ba.Author.AuthorId,
						FirstName = ba.Author.FirstName,
						LastName = ba.Author.LastName,
					}).ToList()
				})
				.ToListAsync();

			return Ok(books);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<BookDisplayDTO>> GetBookById(int id)
		{
			var book = await _context.Books
				.Include(b => b.BookAuthors)
				.ThenInclude(ba => ba.Author)
				.FirstOrDefaultAsync(b => b.BookId == id);

			if (book == null)
			{
				return NotFound();
			}

			var bookDTO = new BookDisplayDTO
			{
				BookId = book.BookId,
				Title = book.Title,
				ISBN = book.ISBN,
				YearPublished = book.YearPublished,
				Rating = book.Rating,
				Authors = book.BookAuthors.Select(ba => new AuthorDisplayDTO
				{
					AuthorId = ba.Author.AuthorId,
					FirstName = ba.Author.FirstName,
					LastName = ba.Author.LastName
				}).ToList()
			};

			return bookDTO;
		}

		[HttpPost]
		public async Task<ActionResult<Book>> PostBook(BookCreateDTO bookDTO)
		{
			var book = new Book
			{
				Title = bookDTO.Title,
				ISBN = bookDTO.ISBN,
				YearPublished = bookDTO.YearPublished,
				Rating = bookDTO.Rating
			};

			_context.Books.Add(book);
			await _context.SaveChangesAsync();

			if (bookDTO.AuthorIds != null && bookDTO.AuthorIds.Any())
			{
				foreach (var authorId in bookDTO.AuthorIds)
				{
					var bookAuthor = new BookAuthor
					{
						BookId = book.BookId,
						AuthorId = authorId
					};
					_context.BookAuthors.Add(bookAuthor);
				}
			}

			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, book);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBook(int id)
		{
			var book = await _context.Books.FindAsync(id);
			if (book == null)
			{
				return NotFound();
			}

			_context.Books.Remove(book);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
