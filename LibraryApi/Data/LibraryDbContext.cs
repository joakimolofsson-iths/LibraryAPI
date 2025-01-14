﻿using LibraryApi.Models;
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
	}
}
