namespace LibraryApi.Models
{
	public class BookCopy
	{
		public int BookCopyId { get; set; }
		public int BookId { get; set; }
		public Book Book { get; set; } = null!;
	}
}
