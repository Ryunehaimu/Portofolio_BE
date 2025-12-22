using Microsoft.EntityFrameworkCore;
using PortfolioBackend.Models;

// PERHATIKAN BARIS INI: Harus ada .Data
namespace PortfolioBackend.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Page> Pages { get; set; }
    }
}