using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext

    //DbContext es una clase propia de .net
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>().HasKey(x => new {x.RoleId, x.AppUserId});
        }

        /*Tablas*/
        public DbSet<Document> Documents { get; set; }
        public DbSet<AppUser> Users { get; set; }
        
        public  DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<DocumentDetail> DocumentDetails { get; set; }
    }
}