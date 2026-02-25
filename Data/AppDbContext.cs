using BibliotecaVirtual.Models;
using BibliotecaVirtual.Shared;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaVirtual.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
