using Moq;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;

namespace Reimbursement_testing
{
    public class ReportServiceTests
    {
        private readonly Mock<IReimbursementRequestRepository> _repoMock;
        private readonly ReportService _service;

        public ReportServiceTests()
        {
            _repoMock = new Mock<IReimbursementRequestRepository>();
            _service = new ReportService(_repoMock.Object);
        }

        private static ReimbursementRequest MakeRequest(
            ReimbursementStatusType status,
            DateTime? createdAt = null,
            string roleName = "Employee")
        {
            var userId = Guid.NewGuid();
            var role = new Role { RoleId = Guid.NewGuid(), RoleName = roleName, CreatedAt = DateTime.UtcNow };
            var user = new User
            {
                UserId = userId,
                FirstName = "Test",
                LastName = "User",
                Email = "test@test.com",
                PasswordHash = "",
                CreatedAt = DateTime.UtcNow,
                Role = role,
                RoleId = role.RoleId
            };

            return new ReimbursementRequest
            {
                ReimbursementRequestId = Guid.NewGuid(),
                UserId = userId,
                User = user,
                ExpenseCategoryId = Guid.NewGuid(),
                Amount = 1000m,
                Description = "Test",
                Status = status,
                AttachmentPath = null,
                ExpenseDate = DateTime.UtcNow,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        }

        private static (DateTime from, DateTime to) ThisMonth()
        {
            var from = DateTime.UtcNow.AddDays(-30);
            var to = DateTime.UtcNow.AddDays(1);
            return (from, to);
        }

        // ─── GENERATE REPORT ──────────────────────────────────────────────────────

        [Fact]
        public async Task GenerateReportAsync_ManagerRole_AllStatus_ReturnsSubmittedAndApproved()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted),
                MakeRequest(ReimbursementStatusType.ManagerApproved),
                MakeRequest(ReimbursementStatusType.Rejected),
                MakeRequest(ReimbursementStatusType.Paid)   // excluded for manager
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "all", "manager", 0, 10);

            // Assert — manager sees Submitted, ManagerApproved, Rejected (not Paid)
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GenerateReportAsync_ManagerRole_PendingFilter_ReturnsOnlySubmitted()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted),
                MakeRequest(ReimbursementStatusType.Submitted),
                MakeRequest(ReimbursementStatusType.ManagerApproved)
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "pending", "manager", 0, 10);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal("Submitted", r.Status));
        }

        [Fact]
        public async Task GenerateReportAsync_ManagerRole_ApprovedFilter_ReturnsManagerApproved()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted),
                MakeRequest(ReimbursementStatusType.ManagerApproved),
                MakeRequest(ReimbursementStatusType.ManagerApproved)
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "approved", "manager", 0, 10);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GenerateReportAsync_ManagerRole_RejectedFilter_ReturnsRejected()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Rejected),
                MakeRequest(ReimbursementStatusType.Submitted)
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "rejected", "manager", 0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("Rejected", result.First().Status);
        }

        [Fact]
        public async Task GenerateReportAsync_FinanceRole_AllStatus_ReturnsFinanceRelevantStatuses()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.ManagerApproved),
                MakeRequest(ReimbursementStatusType.FinanceApproved),
                MakeRequest(ReimbursementStatusType.Paid),
                MakeRequest(ReimbursementStatusType.Rejected),
                MakeRequest(ReimbursementStatusType.Submitted)  // excluded for finance
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "all", "finance", 0, 10);

            // Assert — finance sees ManagerApproved, FinanceApproved, Paid, Rejected
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public async Task GenerateReportAsync_FinanceRole_FinancePendingFilter_ReturnsManagerApproved()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.ManagerApproved),
                MakeRequest(ReimbursementStatusType.FinanceApproved)
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "financepending", "finance", 0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("ManagerApproved", result.First().Status);
        }

        [Fact]
        public async Task GenerateReportAsync_FinanceRole_FinanceApprovedFilter_ReturnsFinanceApproved()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.FinanceApproved),
                MakeRequest(ReimbursementStatusType.ManagerApproved)
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "financeapproved", "finance", 0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("FinanceApproved", result.First().Status);
        }

        [Fact]
        public async Task GenerateReportAsync_FinanceRole_FinancePaidFilter_ReturnsPaid()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Paid),
                MakeRequest(ReimbursementStatusType.FinanceApproved)
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "financepaid", "finance", 0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("Paid", result.First().Status);
        }

        [Fact]
        public async Task GenerateReportAsync_FinanceRole_FinanceRejectedFilter_ReturnsRejected()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Rejected),
                MakeRequest(ReimbursementStatusType.Paid)
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "financerejected", "finance", 0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal("Rejected", result.First().Status);
        }

        [Fact]
        public async Task GenerateReportAsync_HeadRole_ReturnsOnlyManagerAndFinanceRoleUsers()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted, roleName: "Manager"),
                MakeRequest(ReimbursementStatusType.Submitted, roleName: "Finance"),
                MakeRequest(ReimbursementStatusType.Submitted, roleName: "Employee") // excluded
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "all", "head", 0, 10);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GenerateReportAsync_DateFilter_ExcludesOutOfRangeRequests()
        {
            // Arrange
            var from = DateTime.UtcNow.AddDays(-10);
            var to = DateTime.UtcNow;

            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted, createdAt: DateTime.UtcNow.AddDays(-5)),   // in range
                MakeRequest(ReimbursementStatusType.Submitted, createdAt: DateTime.UtcNow.AddDays(-20))   // out of range
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "all", "manager", 0, 10);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GenerateReportAsync_Pagination_RespectsPageSize()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = Enumerable.Range(0, 10)
                .Select(_ => MakeRequest(ReimbursementStatusType.Submitted))
                .ToList();
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "pending", "manager", 0, 3);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GenerateReportAsync_Pagination_SecondPage_ReturnsNextItems()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = Enumerable.Range(0, 10)
                .Select(_ => MakeRequest(ReimbursementStatusType.Submitted))
                .ToList();
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var page1 = (await _service.GenerateReportAsync(from, to, "pending", "manager", 0, 3)).ToList();
            var page2 = (await _service.GenerateReportAsync(from, to, "pending", "manager", 1, 3)).ToList();

            // Assert
            Assert.Equal(3, page1.Count);
            Assert.Equal(3, page2.Count);
            // Pages should have different records
            Assert.DoesNotContain(page2[0].ReimbursementRequestId,
                page1.Select(r => r.ReimbursementRequestId));
        }

        [Fact]
        public async Task GenerateReportAsync_NoRequests_ReturnsEmpty()
        {
            // Arrange
            var (from, to) = ThisMonth();
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ReimbursementRequest>());

            // Act
            var result = await _service.GenerateReportAsync(from, to, "all", "manager", 0, 10);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GenerateReportAsync_MapsEmployeeNameCorrectly()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted)
            };
            requests[0].User!.FirstName = "Alice";
            requests[0].User!.LastName = "Wonder";

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var result = await _service.GenerateReportAsync(from, to, "pending", "manager", 0, 10);

            // Assert
            Assert.Equal("Alice Wonder", result.First().EmployeeName);
        }

        // ─── GET TOTAL COUNT ──────────────────────────────────────────────────────

        [Fact]
        public async Task GetTotalCountAsync_ManagerRole_AllStatus_ReturnsCorrectCount()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted),
                MakeRequest(ReimbursementStatusType.ManagerApproved),
                MakeRequest(ReimbursementStatusType.Rejected),
                MakeRequest(ReimbursementStatusType.Paid)  // excluded
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var count = await _service.GetTotalCountAsync(from, to, "all", "manager");

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task GetTotalCountAsync_FinanceRole_AllStatus_ReturnsCorrectCount()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.ManagerApproved),
                MakeRequest(ReimbursementStatusType.FinanceApproved),
                MakeRequest(ReimbursementStatusType.Paid),
                MakeRequest(ReimbursementStatusType.Rejected),
                MakeRequest(ReimbursementStatusType.Submitted)  // excluded
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var count = await _service.GetTotalCountAsync(from, to, "all", "finance");

            // Assert
            Assert.Equal(4, count);
        }

        [Fact]
        public async Task GetTotalCountAsync_ManagerRole_PendingFilter_ReturnsSubmittedCount()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted),
                MakeRequest(ReimbursementStatusType.Submitted),
                MakeRequest(ReimbursementStatusType.ManagerApproved)
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var count = await _service.GetTotalCountAsync(from, to, "pending", "manager");

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetTotalCountAsync_HeadRole_ReturnsOnlyManagerAndFinanceUsers()
        {
            // Arrange
            var (from, to) = ThisMonth();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(ReimbursementStatusType.Submitted, roleName: "Manager"),
                MakeRequest(ReimbursementStatusType.Submitted, roleName: "Finance"),
                MakeRequest(ReimbursementStatusType.Submitted, roleName: "Employee")
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            // Act
            var count = await _service.GetTotalCountAsync(from, to, "all", "head");

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetTotalCountAsync_NoRequests_ReturnsZero()
        {
            // Arrange
            var (from, to) = ThisMonth();
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ReimbursementRequest>());

            // Act
            var count = await _service.GetTotalCountAsync(from, to, "all", "manager");

            // Assert
            Assert.Equal(0, count);
        }
    }
}
