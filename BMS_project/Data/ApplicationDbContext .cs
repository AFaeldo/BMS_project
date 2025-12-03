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
        public DbSet<Barangay> barangays { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAllocation> ProjectAllocations { get; set; }
        public DbSet<ProjectLog> ProjectLogs { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<KabataanTermPeriod> KabataanTermPeriods { get; set; }
        public DbSet<KabataanServiceRecord> KabataanServiceRecords { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<FederationFund> FederationFunds { get; set; }
        public DbSet<Compliance> Compliances { get; set; }
        public DbSet<Sitio> Sitios { get; set; }
        public DbSet<Announcement> Announcements { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map each entity to the actual table name in your DB
            modelBuilder.Entity<Login>().ToTable("login").HasKey(l => l.Id);
            modelBuilder.Entity<Role>().ToTable("role").HasKey(r => r.Role_ID);
            modelBuilder.Entity<YouthMember>().ToTable("youth_member").HasKey(y => y.Member_ID);
            modelBuilder.Entity<Barangay>().ToTable("barangay").HasKey(b => b.Barangay_ID);
            modelBuilder.Entity<User>().ToTable("user").HasKey(u => u.User_ID); // Changed to "user"
            modelBuilder.Entity<KabataanTermPeriod>().ToTable("kabataan_term_period").HasKey(t => t.Term_ID);
            modelBuilder.Entity<KabataanServiceRecord>().ToTable("kabataan_service_record").HasKey(r => r.Record_ID);
            modelBuilder.Entity<SystemLog>().ToTable("system_log").HasKey(s => s.SysLog_id);
            modelBuilder.Entity<FederationFund>().ToTable("federation_fund").HasKey(f => f.Fund_ID);
            modelBuilder.Entity<Compliance>().ToTable("compliance").HasKey(c => c.Compliance_ID);
            modelBuilder.Entity<Sitio>().ToTable("sitio").HasKey(s => s.Sitio_ID);
            modelBuilder.Entity<Announcement>().ToTable("announcement").HasKey(a => a.Announcement_ID);

            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.User_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sitio>()
                .HasOne(s => s.Barangay)
                .WithMany()
                .HasForeignKey(s => s.Barangay_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<YouthMember>()
                .HasOne(y => y.Sitio)
                .WithMany()
                .HasForeignKey(y => y.Sitio_ID)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Compliance>()
                .HasOne(c => c.KabataanTermPeriod)
                .WithMany()
                .HasForeignKey(c => c.Term_ID)
                .HasConstraintName("fk_term_id")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SystemLog>()
                .HasOne(s => s.User)
                .WithMany() // Or WithMany(u => u.SystemLogs) if added to User
                .HasForeignKey(s => s.User_ID)
                .HasConstraintName("fk_User_Id"); // Match constraint name from SQL dump


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
                .OnDelete(DeleteBehavior.Restrict);

            // --- User -> Role (optional) ---
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()                       // if Role.Users not defined
                .HasForeignKey(u => u.Role_ID)
                .HasConstraintName("fk_user_role")
                .OnDelete(DeleteBehavior.Restrict);

            // Add any additional configuration (indexes, lengths) if needed.

            modelBuilder.Entity<Budget>()
                .ToTable("budget")                    // actual table name in MySQL
                .HasKey(b => b.Budget_ID);

            // Budget -> Barangay
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Barangay)
                .WithMany()                           // if Barangay doesn't have ICollection<Budget>
                .HasForeignKey(b => b.Barangay_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Project ---
            modelBuilder.Entity<Project>()
                .ToTable("project")
                .HasKey(p => p.Project_ID);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.User_ID)
                .OnDelete(DeleteBehavior.Cascade); // project_ibfk_1

            // --- Project Allocation ---
            modelBuilder.Entity<ProjectAllocation>()
                .ToTable("project_allocation")
                .HasKey(pa => pa.Allocation_ID);

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Budget)
                .WithMany()
                .HasForeignKey(pa => pa.Budget_ID)
                .OnDelete(DeleteBehavior.Cascade); // fk_alloc_budget

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(pa => pa.Project)
                .WithMany(p => p.Allocations)
                .HasForeignKey(pa => pa.Project_ID)
                .OnDelete(DeleteBehavior.Cascade); // fk_alloc_project

            // --- Project Log ---
            modelBuilder.Entity<ProjectLog>()
                .ToTable("project_log")
                .HasKey(pl => pl.Log_ID);

            modelBuilder.Entity<ProjectLog>()
                .HasOne(pl => pl.Project)
                .WithMany(p => p.Logs)
                .HasForeignKey(pl => pl.Project_ID)
                .OnDelete(DeleteBehavior.Cascade); // project_log_ibfk_1

            modelBuilder.Entity<ProjectLog>()
                .HasOne(pl => pl.User)
                .WithMany()
                .HasForeignKey(pl => pl.User_ID)
                .OnDelete(DeleteBehavior.SetNull); // project_log_ibfk_2

            // --- File Upload ---
            modelBuilder.Entity<FileUpload>()
                .ToTable("file_upload")
                .HasKey(f => f.File_ID);

            modelBuilder.Entity<FileUpload>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.User_ID)
                .OnDelete(DeleteBehavior.SetNull); // file_upload_ibfk_2

        }
    }
}
