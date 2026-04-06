using Moq;
using ReimbursementTrackerApp.DataTransferObjects.Approval;
using ReimbursementTrackerApp.Models.Approval;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;
using ReimbursementTrackerApp.Services.Interfaces;

namespace Reimbursement_testing
{
    public class ApprovalServiceTests
    {
        private readonly Mock<IApprovalRepository> _approvalRepoMock;
        private readonly Mock<IReimbursementRequestRepository> _requestRepoMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly ApprovalService _service;

        public ApprovalServiceTests()
        {
            _approvalRepoMock = new Mock<IApprovalRepository>();
            _requestRepoMock = new Mock<IReimbursementRequestRepository>();
            _notificationServiceMock = new Mock<INotificationService>();

            _service = new ApprovalService(
                _approvalRepoMock.Object,
                _requestRepoMock.Object,
                _notificationServiceMock.Object);
        }

        private static ReimbursementRequest MakeRequest(
            ReimbursementStatusType status = ReimbursementStatusType.Submitted,
            Guid? id = null,
            Guid? userId = null)
        {
            return new ReimbursementRequest
            {
                ReimbursementRequestId = id ?? Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                ExpenseCategoryId = Guid.NewGuid(),
                Amount = 1500m,
                Description = "Test",
                Status = status,
                AttachmentPath = null,
                ExpenseDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task ProcessApprovalAsync_ValidRequest_ApprovesSuccessfully()
        {
            // Arrange
            var req = MakeRequest(ReimbursementStatusType.Submitted);
            var approverId = Guid.NewGuid();

            _requestRepoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _approvalRepoMock.Setup(r => r.AddAsync(It.IsAny<ApprovalHistory>())).Returns(Task.CompletedTask);
            _requestRepoMock.Setup(r => r.UpdateAsync(req)).Returns(Task.CompletedTask);
            _approvalRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var dto = new ApprovalActionRequestDto
            {
                ReimbursementRequestId = req.ReimbursementRequestId,
                Status = ReimbursementStatusType.ManagerApproved,
                Comments = "Looks good"
            };

            // Act
            await _service.ProcessApprovalAsync(approverId, dto);

            // Assert
            Assert.Equal(ReimbursementStatusType.ManagerApproved, req.Status);
            _approvalRepoMock.Verify(r => r.AddAsync(It.IsAny<ApprovalHistory>()), Times.Once);
            _approvalRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessApprovalAsync_RequestNotFound_ThrowsException()
        {
            // Arrange
            _requestRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ReimbursementRequest?)null);

            var dto = new ApprovalActionRequestDto
            {
                ReimbursementRequestId = Guid.NewGuid(),
                Status = ReimbursementStatusType.ManagerApproved
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.ProcessApprovalAsync(Guid.NewGuid(), dto));
            Assert.Equal("Request not found.", ex.Message);
        }

        [Fact]
        public async Task ProcessApprovalAsync_Rejection_SendsRejectionNotification()
        {
            // Arrange
            var req = MakeRequest(ReimbursementStatusType.Submitted);
            var approverId = Guid.NewGuid();
            string? capturedMessage = null;

            _requestRepoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _approvalRepoMock.Setup(r => r.AddAsync(It.IsAny<ApprovalHistory>())).Returns(Task.CompletedTask);
            _requestRepoMock.Setup(r => r.UpdateAsync(req)).Returns(Task.CompletedTask);
            _approvalRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock
                .Setup(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback<Guid, string>((_, msg) => capturedMessage = msg)
                .Returns(Task.CompletedTask);

            var dto = new ApprovalActionRequestDto
            {
                ReimbursementRequestId = req.ReimbursementRequestId,
                Status = ReimbursementStatusType.Rejected,
                Comments = "Not valid"
            };

            // Act
            await _service.ProcessApprovalAsync(approverId, dto);

            // Assert
            Assert.Contains("Rejected", capturedMessage);
        }

        [Fact]
        public async Task ProcessApprovalAsync_FinanceApproved_SendsCorrectNotification()
        {
            // Arrange
            var req = MakeRequest(ReimbursementStatusType.ManagerApproved);
            string? capturedMessage = null;

            _requestRepoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _approvalRepoMock.Setup(r => r.AddAsync(It.IsAny<ApprovalHistory>())).Returns(Task.CompletedTask);
            _requestRepoMock.Setup(r => r.UpdateAsync(req)).Returns(Task.CompletedTask);
            _approvalRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock
                .Setup(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback<Guid, string>((_, msg) => capturedMessage = msg)
                .Returns(Task.CompletedTask);

            var dto = new ApprovalActionRequestDto
            {
                ReimbursementRequestId = req.ReimbursementRequestId,
                Status = ReimbursementStatusType.FinanceApproved
            };

            // Act
            await _service.ProcessApprovalAsync(Guid.NewGuid(), dto);

            // Assert
            Assert.Contains("Finance Approved", capturedMessage);
        }

        [Fact]
        public async Task ProcessApprovalAsync_SavesApprovalHistoryRecord()
        {
            // Arrange
            var req = MakeRequest();
            var approverId = Guid.NewGuid();
            ApprovalHistory? savedHistory = null;

            _requestRepoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _approvalRepoMock.Setup(r => r.AddAsync(It.IsAny<ApprovalHistory>()))
                .Callback<ApprovalHistory>(h => savedHistory = h)
                .Returns(Task.CompletedTask);
            _requestRepoMock.Setup(r => r.UpdateAsync(req)).Returns(Task.CompletedTask);
            _approvalRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var dto = new ApprovalActionRequestDto
            {
                ReimbursementRequestId = req.ReimbursementRequestId,
                Status = ReimbursementStatusType.ManagerApproved,
                Comments = "Approved"
            };

            // Act
            await _service.ProcessApprovalAsync(approverId, dto);

            // Assert
            Assert.NotNull(savedHistory);
            Assert.Equal(approverId, savedHistory!.ApproverUserId);
            Assert.Equal("Approved", savedHistory.Comments);
        }

        [Fact]
        public async Task GetApprovalHistoryAsync_ReturnsHistory()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var approverUser = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = "Manager",
                LastName = "One",
                Email = "mgr@test.com",
                PasswordHash = "",
                CreatedAt = DateTime.UtcNow
            };

            var histories = new List<ApprovalHistory>
            {
                new ApprovalHistory
                {
                    ApprovalHistoryId = Guid.NewGuid(),
                    ReimbursementRequestId = requestId,
                    ApproverUserId = approverUser.UserId,
                    ApproverUser = approverUser,
                    ApprovalStage = ApprovalStageType.Manager,
                    Status = ReimbursementStatusType.ManagerApproved,
                    Comments = "Good",
                    ActionDate = DateTime.UtcNow
                }
            };

            _approvalRepoMock.Setup(r => r.GetByRequestIdAsync(requestId)).ReturnsAsync(histories);

            // Act
            var result = await _service.GetApprovalHistoryAsync(requestId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Manager One", result.First().ApproverName);
            Assert.Equal("Good", result.First().Comments);
        }

        [Fact]
        public async Task GetApprovalHistoryAsync_NoHistory_ReturnsEmpty()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            _approvalRepoMock.Setup(r => r.GetByRequestIdAsync(requestId))
                .ReturnsAsync(new List<ApprovalHistory>());

            // Act
            var result = await _service.GetApprovalHistoryAsync(requestId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetApprovalHistoryAsync_NullApproverUser_ReturnsUnknown()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var histories = new List<ApprovalHistory>
            {
                new ApprovalHistory
                {
                    ApprovalHistoryId = Guid.NewGuid(),
                    ReimbursementRequestId = requestId,
                    ApproverUserId = Guid.NewGuid(),
                    ApproverUser = null,
                    ApprovalStage = ApprovalStageType.Manager,
                    Status = ReimbursementStatusType.Rejected,
                    ActionDate = DateTime.UtcNow
                }
            };

            _approvalRepoMock.Setup(r => r.GetByRequestIdAsync(requestId)).ReturnsAsync(histories);

            // Act
            var result = await _service.GetApprovalHistoryAsync(requestId);

            // Assert
            Assert.Equal("Unknown", result.First().ApproverName);
        }
    }
}
