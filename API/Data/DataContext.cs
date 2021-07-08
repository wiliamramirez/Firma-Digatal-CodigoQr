using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        /*Tablas*/
        public DbSet<Document> Documents { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}