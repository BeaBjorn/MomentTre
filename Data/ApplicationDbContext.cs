// Using directive for Entity Framework Core
using Microsoft.EntityFrameworkCore;
// Using directive for the models in the application
using MomentTre.Models;

//Namespace
namespace MomentTre.Data;

// Adding DbContext to store the Author and Book tables in the database
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    //DbSet for the Author table
    public DbSet<Author> Authors { get; set; }
    //DbSet for the Book table
    public DbSet<Book> Books { get; set; }
}
