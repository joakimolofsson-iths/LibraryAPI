
using LibraryApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace LibraryApi
{
	public class Program
    {
        public static void Main(string[] args)
        {
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			Console.WriteLine($"Current Environment: {environment}");
			//$env:ASPNETCORE_ENVIRONMENT="Development"

			var builder = WebApplication.CreateBuilder(args);

			builder.Configuration.AddUserSecrets<Program>();
			var dbPassword = builder.Configuration["AzureDbPassword"];
			string connectionString;

			// Choose connection string based on environment
			if (environment == "Development")
			{
				connectionString = builder.Configuration.GetConnectionString("BooksDb");
			}
			else if (environment == "Azure")
			{
				connectionString = builder.Configuration.GetConnectionString("BooksDb")
					.Replace("*****", dbPassword);
			}
			else
			{
				throw new Exception("Unknown environment");
			}

			// Add services to the container.
			builder.Services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
			});

			builder.Services.AddDbContext<LibraryDbContext>(options =>
				options.UseSqlServer(connectionString)
			);

			builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

			if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Azure")
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
				});
			}

			app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
