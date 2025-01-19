using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data
{
	public class LibraryDbContext : DbContext
	{
		public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
		: base(options)
		{
		}

		public DbSet<Author> Authors { get; set; }
		public DbSet<Book> Books { get; set; }
		public DbSet<BookAuthor> BookAuthors { get; set; }
		public DbSet<BookCopy> BookCopies { get; set; }
		public DbSet<Member> Members { get; set; }
		public DbSet<Loan> Loans { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Book>()
				.HasIndex(b => b.ISBN)
				.IsUnique();

			modelBuilder.Entity<BookAuthor>()
				.HasKey(ba => new { ba.BookId, ba.AuthorId });

			modelBuilder.Entity<BookAuthor>()
				.HasOne(ba => ba.Book)
				.WithMany(b => b.BookAuthors)
				.HasForeignKey(ba => ba.BookId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<BookAuthor>()
				.HasOne(ba => ba.Author)
				.WithMany(a => a.BookAuthors)
				.HasForeignKey(ba => ba.AuthorId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Member>()
			   .HasIndex(m => m.CardNumber)
			   .IsUnique();
		}
	}
}
