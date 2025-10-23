using BMS_project.Models;
using Microsoft.EntityFrameworkCore;

namespace BMS_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Login> Login { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Schema and table mapping
            modelBuilder.Entity<Login>().ToTable("login");
            modelBuilder.Entity<Role>().ToTable("role");

            // Define relationship between login and role
            modelBuilder.Entity<Login>()
                .HasOne(l => l.Role)
                .WithMany(r => r.Logins)
                .HasForeignKey(l => l.Role_ID);
        }
    }
}
