namespace LibraryApi.Models
{
	public class Loan
	{
		public int LoanId { get; set; }
		public int BookCopyId { get; set; }
		public BookCopy BookCopy { get; set; } = null!;
		public int MemberId { get; set; }
		public Member Member { get; set; } = null!;
		public DateTime LoanDate { get; set; }
		public DateTime? ReturnDate { get; set; }
	}
}
