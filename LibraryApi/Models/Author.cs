namespace LibraryApi.Models
{
	public class Author
	{
		public int AuthorId { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
	}
}
