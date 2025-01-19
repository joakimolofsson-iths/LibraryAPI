using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MembersController : ControllerBase
	{
		private readonly LibraryDbContext _context;
		public MembersController(LibraryDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<MemberDisplayDTO>>> GetAllMembers()
		{
			var members = await _context.Members
				.Select(a => new MemberDisplayDTO
				{
					MemberId = a.MemberId,
					FirstName = a.FirstName,
					LastName = a.LastName,
					CardNumber = a.CardNumber
				})
				.ToListAsync();

			var member = await _context.Members.ToListAsync();
			return Ok(member);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Member>> GetMemberById(int id)
		{
			var member = await _context.Members.FindAsync(id);
			if (member == null)
			{
				return NotFound();
			}

			return member;
		}

		[HttpPost]
		public async Task<ActionResult<Member>> PostMember(MemberCreateDTO memberDTO)
		{
			var member = new Member
			{
				FirstName = memberDTO.FirstName,
				LastName = memberDTO.LastName
			};

			_context.Members.Add(member);
			await _context.SaveChangesAsync();

			var date = DateTime.Now.ToString("yyyyMMdd");
			member.CardNumber = $"{date}{member.MemberId}";

			_context.Entry(member).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetMemberById), new { id = member.MemberId }, member);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMember(int id)
		{
			var member = await _context.Members.FindAsync(id);
			if (member == null)
			{
				return NotFound();
			}

			_context.Members.Remove(member);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
