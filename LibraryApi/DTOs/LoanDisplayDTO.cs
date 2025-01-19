namespace LibraryApi.DTOs
{
	public class LoanDisplayDTO
	{
		public int LoanId { get; set; }
		public int BookCopyId { get; set; }
		public int MemberId { get; set; }
		public DateTime LoanDate { get; set; }
		public DateTime? ReturnDate { get; set; }
	}
}
