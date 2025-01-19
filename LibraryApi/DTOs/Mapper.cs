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

		public static MemberDisplayAllDTO ToMemberDisplayAllDTO(Member member)
		{
			return new MemberDisplayAllDTO
			{
				MemberId = member.MemberId,
				FirstName = member.FirstName,
				LastName = member.LastName,
				CardNumber = member.CardNumber
			};
		}

		public static MemberDisplayDTO ToMemberDisplayDTO(Member member)
		{
			return new MemberDisplayDTO
			{
				MemberId = member.MemberId,
				FirstName = member.FirstName,
				LastName = member.LastName,
				CardNumber = member.CardNumber,
				Loans = member.Loans.Select(lo => new LoanDisplayDTO
				{
					LoanId = lo.LoanId,
					BookCopyId = lo.BookCopyId,
					MemberId = lo.MemberId,
					LoanDate = lo.LoanDate,
					ReturnDate = lo.ReturnDate
				}).ToList()
			};
		}

		public static LoanDisplayDTO ToLoanDisplayDTO(Loan loan)
		{
			return new LoanDisplayDTO
			{
				LoanId = loan.LoanId,
				BookCopyId = loan.BookCopyId,
				MemberId = loan.MemberId,
				LoanDate = loan.LoanDate,
				ReturnDate = loan.ReturnDate
			};
		}
	}
}
