namespace LibraryApi.Models
{
	public class Member
	{
		public int MemberId { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public string? CardNumber { get; set; }
		public ICollection<Loan> Loans { get; set; } = new List<Loan>();
	}
}
