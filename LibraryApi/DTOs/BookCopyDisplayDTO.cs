using LibraryApi.Models;

namespace LibraryApi.DTOs
{
	public class BookCopyDisplayDTO
	{
		public int BookCopyId { get; set; }
		public int BookId { get; set; }
		public required string Title { get; set; }
		public bool OnLoan { get; set; }
	}
}
