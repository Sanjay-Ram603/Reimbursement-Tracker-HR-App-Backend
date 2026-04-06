using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Models.Approval;
using ReimbursementTrackerApp.Models.Audit;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Models.Notification;
using ReimbursementTrackerApp.Models.Payment;
using ReimbursementTrackerApp.Models.Policy;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Models.UserManagement;


namespace ReimbursementTrackerApp.Contexts
{
    public class ReimbursementDbContext : DbContext
    {
        public ReimbursementDbContext(DbContextOptions<ReimbursementDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<BankDetail> BankDetails { get; set; }

        
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<ReimbursementRequest> ReimbursementRequests { get; set; }
        public DbSet<ReimbursementAttachment> ReimbursementAttachments { get; set; }
        public DbSet<ReimbursementStatusHistory> ReimbursementStatusHistories { get; set; }



        
        public DbSet<ApprovalHistory> ApprovalHistories { get; set; }

        public DbSet<PaymentRecord> PaymentRecords { get; set; }

        public DbSet<PolicyRule> PolicyRules { get; set; }
        public DbSet<PolicyViolation> PolicyViolations { get; set; }

        
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApprovalHistory>()
        .HasOne(a => a.ApproverUser)
        .WithMany()
        .HasForeignKey(a => a.ApproverUserId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReimbursementStatusHistory>()
                .HasOne(r => r.ChangedByUser)
                .WithMany()
                .HasForeignKey(r => r.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

           

            modelBuilder.Entity<ReimbursementRequest>()
                .Property(r => r.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PaymentRecord>()
                .Property(p => p.AmountPaid)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PolicyRule>()
                .Property(p => p.MaximumAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ExpenseCategory>().HasData(
        new ExpenseCategory
        {
            ExpenseCategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CategoryName = "Travel"
        },
        new ExpenseCategory
        {
            ExpenseCategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            CategoryName = "Food"
        },
        new ExpenseCategory
        {
            ExpenseCategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            CategoryName = "Accommodation"
        }
    );
        }
    }
}
