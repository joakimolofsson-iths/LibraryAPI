namespace LibraryApi.DTOs
{
	public class MemberDisplayDTO
	{
		public int MemberId { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public string? CardNumber { get; set; }
	}
}
