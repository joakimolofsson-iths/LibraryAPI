namespace LibraryApi.DTOs
{
	public class BookDisplayDTO
	{
		public int BookId { get; set; }
		public required string Title { get; set; }
		public required string ISBN { get; set; }
		public int YearPublished { get; set; }
		public int Rating { get; set; }
		public List<AuthorDisplayDTO> Authors { get; set; } = new List<AuthorDisplayDTO>();
	}
}
