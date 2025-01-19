namespace LibraryApi.Models
{
	public class Book
	{
		public int BookId { get; set; }
		public required string Title { get; set; }
		public required string ISBN { get; set; }
		public int YearPublished { get; set; }
		public int Rating { get; set; }
		public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
		public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
	}
}
