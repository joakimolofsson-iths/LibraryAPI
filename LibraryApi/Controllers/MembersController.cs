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
		public async Task<ActionResult<IEnumerable<MemberDisplayAllDTO>>> GetAllMembers()
		{
			var members = await _context.Members.ToListAsync();
			var membersDTOs = members.Select(Mapper.ToMemberDisplayAllDTO).ToList();

			return Ok(membersDTOs);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<MemberDisplayDTO>> GetMemberById(int id)
		{
			var member = await _context.Members
				.Include(m => m.Loans)
				.FirstOrDefaultAsync(m => m.MemberId == id);

			if (member == null)
			{
				return NotFound();
			}

			var memberDisplayDTO = Mapper.ToMemberDisplayDTO(member);

			return memberDisplayDTO;
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

			var memberDisplayAllDTO = Mapper.ToMemberDisplayAllDTO(member);

			return CreatedAtAction(nameof(GetMemberById), new { id = member.MemberId }, memberDisplayAllDTO);
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

			return Ok(new {message="Member deleted successfully..."});
		}
	}
}
