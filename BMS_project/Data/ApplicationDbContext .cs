using Microsoft.EntityFrameworkCore;
using BMS_project.Models;

namespace BMS_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Login> Login { get; set; }
        public DbSet<Role> Role { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>().ToTable("login").HasKey(l => l.Id);
            modelBuilder.Entity<Role>().ToTable("role").HasKey(r => r.Role_ID);

            modelBuilder.Entity<Login>()
                .HasOne(l => l.Role)
                .WithMany(r => r.Logins)
                .HasForeignKey(l => l.Role_ID)
                .HasConstraintName("fk_login_role"); // optional, for clarity
        }
    }
}
