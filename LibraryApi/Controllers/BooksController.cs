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
		public async Task<ActionResult<IEnumerable<BookDisplayAllDTO>>> GetAllBooks()
		{
			var books = await _context.Books.ToListAsync();
			var bookDTOs = books.Select(Mapper.ToBookDisplayAllDTO).ToList();

			return Ok(bookDTOs);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<BookDisplayDTO>> GetBookById(int id)
		{
			var book = await _context.Books
				.Include(b => b.BookAuthors)
				.ThenInclude(ba => ba.Author)
				.Include(b => b.BookCopies)
				.FirstOrDefaultAsync(b => b.BookId == id);

			if (book == null)
			{
				return NotFound();
			}

			var bookDisplayDTO = Mapper.ToBookDisplayDTO(book);

			return bookDisplayDTO;
		}

		[HttpPost]
		public async Task<ActionResult<Book>> PostBook(BookCreateDTO bookDTO)
		{
			if (bookDTO.AuthorIds != null && bookDTO.AuthorIds.Any())
			{
				foreach (var authorId in bookDTO.AuthorIds)
				{
					if (!_context.Authors.Any(a => a.AuthorId == authorId))
					{
						return BadRequest("One or more authors do not exist...");
					}
				}
			}

			int copies = bookDTO.Copies > 0 ? bookDTO.Copies : 1;

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

			for (int i = 0; i < copies; i++)
			{
				var bookCopy = new BookCopy
				{
					BookId = book.BookId
				};
				_context.BookCopies.Add(bookCopy);
			}

			await _context.SaveChangesAsync();

			var fullBook = await _context.Books
				.Include(b => b.BookAuthors)
				.ThenInclude(ba => ba.Author)
				.Include(b => b.BookCopies)
				.FirstOrDefaultAsync(b => b.BookId == book.BookId);

			if (fullBook == null)
			{
				return NotFound();
			}

			var bookDisplayDTO = Mapper.ToBookDisplayDTO(fullBook);

			return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, bookDisplayDTO);
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

			return Ok(new { message = "Book deleted successfully..." });
		}
	}
}
