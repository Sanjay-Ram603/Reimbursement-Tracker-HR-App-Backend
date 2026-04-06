using Moq;
using ReimbursementTrackerApp.DataTransferObjects.Reimbursement;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;

namespace Reimbursement_testing
{
    public class ReimbursementServiceTests
    {
        private readonly Mock<IReimbursementRequestRepository> _repoMock;
        private readonly ReimbursementService _service;

        public ReimbursementServiceTests()
        {
            _repoMock = new Mock<IReimbursementRequestRepository>();
            _service = new ReimbursementService(_repoMock.Object);
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
                Amount = 1000m,
                Description = "Test expense",
                Status = status,
                AttachmentPath = "/uploads/test.pdf",
                ExpenseDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                User = new User
                {
                    UserId = userId ?? Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@test.com",
                    PasswordHash = "",
                    CreatedAt = DateTime.UtcNow,
                    Role = new Role { RoleId = Guid.NewGuid(), RoleName = "Employee", CreatedAt = DateTime.UtcNow }
                }
            };
        }

        [Fact]
        public async Task CreateRequestAsync_NoAttachment_CreatesRequest()
        {
          
            var userId = Guid.NewGuid();
            var dto = new CreateReimbursementRequestDto
            {
                ExpenseCategoryId = Guid.NewGuid(),
                Amount = 500m,
                Description = "Lunch",
                ExpenseDate = DateTime.UtcNow,
                Attachment = null
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<ReimbursementRequest>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

      
            var result = await _service.CreateRequestAsync(userId, dto);

      
            Assert.NotEqual(Guid.Empty, result);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<ReimbursementRequest>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateRequestAsync_StatusIsAlwaysSubmitted()
        {
    
            var userId = Guid.NewGuid();
            var dto = new CreateReimbursementRequestDto
            {
                ExpenseCategoryId = Guid.NewGuid(),
                Amount = 200m,
                Description = "Travel",
                ExpenseDate = DateTime.UtcNow,
                Attachment = null
            };

            ReimbursementRequest? saved = null;
            _repoMock.Setup(r => r.AddAsync(It.IsAny<ReimbursementRequest>()))
                .Callback<ReimbursementRequest>(r => saved = r)
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

      
            await _service.CreateRequestAsync(userId, dto);

     
            Assert.NotNull(saved);
            Assert.Equal(ReimbursementStatusType.Submitted, saved!.Status);
        }

        [Fact]
        public async Task GetUserRequestsAsync_ReturnsUserRequests()
        {
     
            var userId = Guid.NewGuid();
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(userId: userId),
                MakeRequest(userId: userId)
            };

            _repoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(requests);

          
            var result = await _service.GetUserRequestsAsync(userId);

       
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserRequestsAsync_EmptyList_ReturnsEmpty()
        {
          
            var userId = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<ReimbursementRequest>());

        
            var result = await _service.GetUserRequestsAsync(userId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsDto()
        {
           
            var req = MakeRequest();
            _repoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);

            var result = await _service.GetByIdAsync(req.ReimbursementRequestId);

        
            Assert.NotNull(result);
            Assert.Equal(req.ReimbursementRequestId, result!.ReimbursementRequestId);
            Assert.Equal(req.Amount, result.Amount);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
         
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ReimbursementRequest?)null);

          
            var result = await _service.GetByIdAsync(Guid.NewGuid());

          
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllRequestsAsync_ReturnsAllRequests()
        {
     
            var requests = new List<ReimbursementRequest>
            {
                MakeRequest(), MakeRequest(), MakeRequest()
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(requests);

            
            var result = await _service.GetAllRequestsAsync();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task UpdateRequestAsync_SubmittedRequest_UpdatesSuccessfully()
        {
          
            var req = MakeRequest(ReimbursementStatusType.Submitted);
            _repoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<ReimbursementRequest>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new UpdateReimbursementStatusRequestDto
            {
                Amount = 2000m,
                Description = "Updated description"
            };

      
            await _service.UpdateRequestAsync(req.ReimbursementRequestId, dto);

       
            Assert.Equal(2000m, req.Amount);
            Assert.Equal("Updated description", req.Description);
            _repoMock.Verify(r => r.UpdateAsync(req), Times.Once);
        }

        [Fact]
        public async Task UpdateRequestAsync_NotFound_ThrowsException()
        {
        
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ReimbursementRequest?)null);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateRequestAsync(Guid.NewGuid(), new UpdateReimbursementStatusRequestDto()));
            Assert.Equal("Request not found.", ex.Message);
        }

        [Fact]
        public async Task UpdateRequestAsync_NonSubmittedStatus_ThrowsException()
        {
   
            var req = MakeRequest(ReimbursementStatusType.ManagerApproved);
            _repoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);


            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateRequestAsync(req.ReimbursementRequestId, new UpdateReimbursementStatusRequestDto()));
            Assert.Contains("Only submitted", ex.Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_ValidRequest_UpdatesStatus()
        {
    
            var req = MakeRequest(ReimbursementStatusType.Submitted);
            _repoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<ReimbursementRequest>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

         
            await _service.UpdateStatusAsync(req.ReimbursementRequestId, ReimbursementStatusType.ManagerApproved);

            
            Assert.Equal(ReimbursementStatusType.ManagerApproved, req.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_PaidRequest_ThrowsException()
        {
          
            var req = MakeRequest(ReimbursementStatusType.Paid);
            _repoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);

          
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateStatusAsync(req.ReimbursementRequestId, ReimbursementStatusType.Rejected));
            Assert.Contains("Paid requests", ex.Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_NotFound_ThrowsException()
        {
          
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ReimbursementRequest?)null);

           
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateStatusAsync(Guid.NewGuid(), ReimbursementStatusType.Rejected));
            Assert.Equal("Request not found.", ex.Message);
        }

        [Fact]
        public async Task DeleteReimbursementRequest_SubmittedRequest_ReturnsTrue()
        {
       
            var req = MakeRequest(ReimbursementStatusType.Submitted);
            _repoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _repoMock.Setup(r => r.Delete(req));
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

          
            var result = await _service.DeleteReimbursementRequest(req.ReimbursementRequestId);

            Assert.True(result);
            _repoMock.Verify(r => r.Delete(req), Times.Once);
        }

        [Fact]
        public async Task DeleteReimbursementRequest_NotFound_ReturnsFalse()
        {
         
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ReimbursementRequest?)null);

          
            var result = await _service.DeleteReimbursementRequest(Guid.NewGuid());

          
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteReimbursementRequest_NonSubmittedStatus_ThrowsException()
        {
          
            var req = MakeRequest(ReimbursementStatusType.ManagerApproved);
            _repoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.DeleteReimbursementRequest(req.ReimbursementRequestId));
            Assert.Contains("Only submitted", ex.Message);
        }
    }
}
