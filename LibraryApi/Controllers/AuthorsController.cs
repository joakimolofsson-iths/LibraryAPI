using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace LibraryApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthorsController : ControllerBase
	{
		private readonly LibraryDbContext _context;
		public AuthorsController(LibraryDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<AuthorDisplayDTO>>> GetAllAuthors()
		{
			var authors = await _context.Authors.ToListAsync();
			var authorDTOs = authors.Select(Mapper.ToAuthorDisplayDTO).ToList();

			return Ok(authorDTOs);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AuthorDisplayDTO>> GetAuthorById(int id)
		{
			var author = await _context.Authors.FindAsync(id);
			if(author == null)
			{
				return NotFound();
			}

			var authorDisplayDTO = Mapper.ToAuthorDisplayDTO(author);

			return authorDisplayDTO;
		}

		[HttpPost]
		public async Task<ActionResult<Author>> PostAuthor(AuthorCreateDTO authorDTO)
		{
			var author = new Author
			{
				FirstName = authorDTO.FirstName,
				LastName = authorDTO.LastName
			};

			_context.Authors.Add(author);
			await _context.SaveChangesAsync();

			var authorDisplayDTO = Mapper.ToAuthorDisplayDTO(author);

			return CreatedAtAction(nameof(GetAuthorById), new { id = author.AuthorId }, authorDisplayDTO); 
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAuthor(int id)
		{
			var author = await _context.Authors
				.Include(a => a.BookAuthors)
				.ThenInclude(ba => ba.Book)
				.FirstOrDefaultAsync(a => a.AuthorId == id);

			if (author == null)
			{
				return NotFound();
			}

			if (author.BookAuthors.Any())
			{
				return BadRequest("This author has books and cannot be deleted....");
			}

			_context.Authors.Remove(author);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
