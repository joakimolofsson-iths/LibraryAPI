using LibraryApi.Models;

namespace LibraryApi.DTOs
{
	public static class Mapper
	{
		public static AuthorDisplayDTO ToAuthorDisplayDTO(Author author)
		{
			return new AuthorDisplayDTO
			{
				AuthorId = author.AuthorId,
				FirstName = author.FirstName,
				LastName = author.LastName
			};
		}

		public static BookDisplayAllDTO ToBookDisplayAllDTO(Book book)
		{
			return new BookDisplayAllDTO
			{
				BookId = book.BookId,
				Title = book.Title
			};
		}

		public static BookDisplayDTO ToBookDisplayDTO(Book book)
		{
			return new BookDisplayDTO
			{
				BookId = book.BookId,
				Title = book.Title,
				ISBN = book.ISBN,
				YearPublished = book.YearPublished,
				Rating = book.Rating,
				Copies = book.BookCopies.Count(),
				Authors = book.BookAuthors.Select(ba => new AuthorDisplayDTO
				{
					AuthorId = ba.Author.AuthorId,
					FirstName = ba.Author.FirstName,
					LastName = ba.Author.LastName
				}).ToList()
			};
		}

		public static BookCopyDisplayDTO ToBookCopyDisplayDTO(BookCopy bookCopy)
		{
			return new BookCopyDisplayDTO
			{
				BookCopyId = bookCopy.BookCopyId,
				BookId = bookCopy.BookId,
				Title = bookCopy.Book.Title,
				OnLoan = bookCopy.OnLoan
			};
		}
	}
}
