using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Services.Implementations;

namespace Reimbursement_testing
{
    public class DashboardServiceTests : IDisposable
    {
        private readonly ReimbursementDbContext _context;
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            var options = new DbContextOptionsBuilder<ReimbursementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test class
                .Options;

            _context = new ReimbursementDbContext(options);
            _service = new DashboardService(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async Task SeedRequestsAsync(Guid userId, List<ReimbursementStatusType> statuses)
        {
            // Seed a role and user first to satisfy FK constraints
            var role = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = "Employee",
                CreatedAt = DateTime.UtcNow
            };
            _context.Roles.Add(role);

            var user = new User
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "User",
                Email = $"{userId}@test.com",
                PasswordHash = "hash",
                RoleId = role.RoleId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);

            var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            foreach (var status in statuses)
            {
                _context.ReimbursementRequests.Add(new ReimbursementRequest
                {
                    ReimbursementRequestId = Guid.NewGuid(),
                    UserId = userId,
                    ExpenseCategoryId = categoryId,
                    Amount = 1000m,
                    Description = "Test",
                    Status = status,
                    AttachmentPath = "/uploads/test.pdf",
                    ExpenseDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        // ─── GET MY DASHBOARD ─────────────────────────────────────────────────────

        [Fact]
        public async Task GetMyDashboardAsync_NoRequests_ReturnsZeroCounts()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _service.GetMyDashboardAsync(userId);

            // Assert
            Assert.Equal(0, result.TotalRequests);
            Assert.Equal(0, result.ApprovedRequests);
            Assert.Equal(0, result.RejectedRequests);
            Assert.Equal(0, result.PendingRequests);
            Assert.Equal(0m, result.TotalAmount);
        }

        [Fact]
        public async Task GetMyDashboardAsync_WithRequests_ReturnsTotalCount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            await SeedRequestsAsync(userId, new List<ReimbursementStatusType>
            {
                ReimbursementStatusType.Submitted,
                ReimbursementStatusType.ManagerApproved,
                ReimbursementStatusType.Rejected
            });

            // Act
            var result = await _service.GetMyDashboardAsync(userId);

            // Assert
            Assert.Equal(3, result.TotalRequests);
        }

        [Fact]
        public async Task GetMyDashboardAsync_CountsPendingCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            await SeedRequestsAsync(userId, new List<ReimbursementStatusType>
            {
                ReimbursementStatusType.Submitted,
                ReimbursementStatusType.Submitted,
                ReimbursementStatusType.ManagerApproved
            });

            // Act
            var result = await _service.GetMyDashboardAsync(userId);

            // Assert
            Assert.Equal(2, result.PendingRequests);
        }

        [Fact]
        public async Task GetMyDashboardAsync_CountsApprovedCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            await SeedRequestsAsync(userId, new List<ReimbursementStatusType>
            {
                ReimbursementStatusType.ManagerApproved,
                ReimbursementStatusType.FinanceApproved,
                ReimbursementStatusType.Submitted
            });

            // Act
            var result = await _service.GetMyDashboardAsync(userId);

            // Assert — both ManagerApproved and FinanceApproved count as approved
            Assert.Equal(2, result.ApprovedRequests);
        }

        [Fact]
        public async Task GetMyDashboardAsync_CountsRejectedCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            await SeedRequestsAsync(userId, new List<ReimbursementStatusType>
            {
                ReimbursementStatusType.Rejected,
                ReimbursementStatusType.Rejected,
                ReimbursementStatusType.Submitted
            });

            // Act
            var result = await _service.GetMyDashboardAsync(userId);

            // Assert
            Assert.Equal(2, result.RejectedRequests);
        }

        [Fact]
        public async Task GetMyDashboardAsync_SumsTotalAmountCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = new Role { RoleId = Guid.NewGuid(), RoleName = "Employee", CreatedAt = DateTime.UtcNow };
            _context.Roles.Add(role);
            var user = new User
            {
                UserId = userId,
                FirstName = "A",
                LastName = "B",
                Email = "ab@test.com",
                PasswordHash = "h",
                RoleId = role.RoleId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);

            var catId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            _context.ReimbursementRequests.AddRange(
                new ReimbursementRequest { ReimbursementRequestId = Guid.NewGuid(), UserId = userId, ExpenseCategoryId = catId, Amount = 500m, Description = "A", Status = ReimbursementStatusType.Submitted, AttachmentPath = "/uploads/a.pdf", ExpenseDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
                new ReimbursementRequest { ReimbursementRequestId = Guid.NewGuid(), UserId = userId, ExpenseCategoryId = catId, Amount = 1500m, Description = "B", Status = ReimbursementStatusType.ManagerApproved, AttachmentPath = "/uploads/b.pdf", ExpenseDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetMyDashboardAsync(userId);

            // Assert
            Assert.Equal(2000m, result.TotalAmount);
        }

        [Fact]
        public async Task GetMyDashboardAsync_OnlyReturnsRequestsForGivenUser()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            await SeedRequestsAsync(userId1, new List<ReimbursementStatusType>
            {
                ReimbursementStatusType.Submitted,
                ReimbursementStatusType.Submitted
            });
            await SeedRequestsAsync(userId2, new List<ReimbursementStatusType>
            {
                ReimbursementStatusType.Submitted
            });

            // Act
            var result = await _service.GetMyDashboardAsync(userId1);

            // Assert — only userId1's 2 requests
            Assert.Equal(2, result.TotalRequests);
        }

        [Fact]
        public async Task GetMyDashboardAsync_AllStatusTypes_CountsCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            await SeedRequestsAsync(userId, new List<ReimbursementStatusType>
            {
                ReimbursementStatusType.Draft,
                ReimbursementStatusType.Submitted,
                ReimbursementStatusType.ManagerApproved,
                ReimbursementStatusType.FinanceApproved,
                ReimbursementStatusType.Rejected,
                ReimbursementStatusType.Paid
            });

            // Act
            var result = await _service.GetMyDashboardAsync(userId);

            // Assert
            Assert.Equal(6, result.TotalRequests);
            Assert.Equal(1, result.PendingRequests);   // Submitted only
            Assert.Equal(2, result.ApprovedRequests);  // ManagerApproved + FinanceApproved
            Assert.Equal(1, result.RejectedRequests);  // Rejected only
        }
    }
}
