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
        public DbSet<YouthMember> YouthMembers { get; set; }
        public DbSet<BarangayProfile> BarangayProfiles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Login>().ToTable("login").HasKey(l => l.Id);
            modelBuilder.Entity<Role>().ToTable("role").HasKey(r => r.Role_ID);

            modelBuilder.Entity<Login>()
                .HasOne(l => l.Role)
                .WithMany(r => r.Logins)
                .HasForeignKey(l => l.Role_ID)
                .HasConstraintName("fk_login_role");

            // 👇 Map YouthMember to your existing table
            modelBuilder.Entity<YouthMember>().ToTable("youth_member").HasKey(y => y.Member_ID);
        }

    }
}
