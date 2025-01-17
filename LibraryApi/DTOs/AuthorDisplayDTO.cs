namespace LibraryApi.DTOs
{
	public class AuthorDisplayDTO
	{
		public int AuthorId { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
	}
}
