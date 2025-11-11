using Microsoft.EntityFrameworkCore;
using BMS_project.Models;

namespace BMS_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSets (use plural property names for clarity)
        public DbSet<Login> Login { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<YouthMember> YouthMembers { get; set; }
        public DbSet<Barangay> Barangays { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map each entity to the actual table name in your DB
            modelBuilder.Entity<Login>().ToTable("login").HasKey(l => l.Id);
            modelBuilder.Entity<Role>().ToTable("role").HasKey(r => r.Role_ID);
            modelBuilder.Entity<YouthMember>().ToTable("youth_member").HasKey(y => y.Member_ID);
            modelBuilder.Entity<Barangay>().ToTable("barangay").HasKey(b => b.Barangay_ID);
            modelBuilder.Entity<User>().ToTable("user").HasKey(u => u.User_ID); // Changed to "user"

            // --- Login -> Role (existing) ---
            modelBuilder.Entity<Login>()
                .HasOne(l => l.Role)
                .WithMany(r => r.Login)            // if Role.Login is a collection
                .HasForeignKey(l => l.Role_ID)
                .HasConstraintName("fk_login_role")
                .OnDelete(DeleteBehavior.Restrict); // choose behavior

            // --- Login -> User (ONE-TO-ONE) ---
            // User model exposes a single Login property, so configure WithOne
            modelBuilder.Entity<Login>()
                .HasOne(l => l.User)
                .WithOne(u => u.Login)            // match User.Login (single)
                .HasForeignKey<Login>(l => l.User_ID)
                .HasConstraintName("fk_login_user")
                .OnDelete(DeleteBehavior.Cascade); // delete logins when user deleted

            // --- User -> Barangay ---
            modelBuilder.Entity<User>()
                .HasOne(u => u.Barangay)
                .WithMany()                       // if you don't have collection on Barangays
                .HasForeignKey(u => u.Barangay_ID)
                .HasConstraintName("fk_user_barangay")
                .OnDelete(DeleteBehavior.Restrict);

            // --- User -> Role (optional) ---
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()                       // if Role.Users not defined
                .HasForeignKey(u => u.Role_ID)
                .HasConstraintName("fk_user_role")
                .OnDelete(DeleteBehavior.Restrict);

            // Add any additional configuration (indexes, lengths) if needed.
        }
    }
}
