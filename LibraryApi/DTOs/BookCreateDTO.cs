namespace LibraryApi.DTOs
{
	public class BookCreateDTO
	{
		public required string Title { get; set; }
		public required string ISBN { get; set; }
		public int YearPublished { get; set; }
		public int Rating { get; set; }
		public int Copies { get; set; }
		public List<int> AuthorIds { get; set; } = new List<int>();
	}
}
