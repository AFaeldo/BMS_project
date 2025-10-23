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
            modelBuilder.Entity<Login>().ToTable("login").HasKey(l => l.Id);
            modelBuilder.Entity<Role>().ToTable("role").HasKey(r => r.Role_ID);

            modelBuilder.Entity<Login>()
                .HasOne<Role>()
                .WithMany()
                .HasForeignKey(l => l.Role_ID);
        }
    }
}
