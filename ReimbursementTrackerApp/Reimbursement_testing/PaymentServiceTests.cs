using Moq;
using ReimbursementTrackerApp.DataTransferObjects.Payment;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Payment;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;
using ReimbursementTrackerApp.Services.Interfaces;

namespace Reimbursement_testing
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepoMock;
        private readonly Mock<IReimbursementRequestRepository> _requestRepoMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly PaymentService _service;

        public PaymentServiceTests()
        {
            _paymentRepoMock = new Mock<IPaymentRepository>();
            _requestRepoMock = new Mock<IReimbursementRequestRepository>();
            _notificationServiceMock = new Mock<INotificationService>();

            _service = new PaymentService(
                _paymentRepoMock.Object,
                _requestRepoMock.Object,
                _notificationServiceMock.Object);
        }

        private static ReimbursementRequest MakeRequest(
            ReimbursementStatusType status = ReimbursementStatusType.FinanceApproved,
            Guid? id = null,
            Guid? userId = null)
        {
            return new ReimbursementRequest
            {
                ReimbursementRequestId = id ?? Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                ExpenseCategoryId = Guid.NewGuid(),
                Amount = 3000m,
                Description = "Test payment",
                Status = status,
                AttachmentPath = null,
                ExpenseDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

    

        [Fact]
        public async Task ProcessPaymentAsync_ValidRequest_ProcessesSuccessfully()
        {
           
            var req = MakeRequest();
            var dto = new ProcessPaymentRequestDto
            {
                ReimbursementRequestId = req.ReimbursementRequestId,
                Amount = req.Amount,
                PaymentMethod = PaymentMethodType.BankTransfer
            };

            _requestRepoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<PaymentRecord>())).Returns(Task.CompletedTask);
            _requestRepoMock.Setup(r => r.UpdateAsync(req)).Returns(Task.CompletedTask);
            _paymentRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            
            await _service.ProcessPaymentAsync(dto);

            
            Assert.Equal(ReimbursementStatusType.Paid, req.Status);
            _paymentRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentAsync_RequestNotFound_ThrowsException()
        {
            
            _requestRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ReimbursementRequest?)null);

            var dto = new ProcessPaymentRequestDto
            {
                ReimbursementRequestId = Guid.NewGuid(),
                Amount = 1000m,
                PaymentMethod = PaymentMethodType.UPI
            };

            
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.ProcessPaymentAsync(dto));
            Assert.Equal("Reimbursement request not found.", ex.Message);
        }

        [Fact]
        public async Task ProcessPaymentAsync_SetsStatusToPaid()
        {
           
            var req = MakeRequest(ReimbursementStatusType.FinanceApproved);
            var dto = new ProcessPaymentRequestDto
            {
                ReimbursementRequestId = req.ReimbursementRequestId,
                Amount = req.Amount,
                PaymentMethod = PaymentMethodType.Cash
            };

            _requestRepoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<PaymentRecord>())).Returns(Task.CompletedTask);
            _requestRepoMock.Setup(r => r.UpdateAsync(req)).Returns(Task.CompletedTask);
            _paymentRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            
            await _service.ProcessPaymentAsync(dto);

          
            Assert.Equal(ReimbursementStatusType.Paid, req.Status);
        }

        [Fact]
        public async Task ProcessPaymentAsync_CreatesPaymentRecord()
        {
        
            var req = MakeRequest();
            PaymentRecord? savedPayment = null;

            var dto = new ProcessPaymentRequestDto
            {
                ReimbursementRequestId = req.ReimbursementRequestId,
                Amount = 2500m,
                PaymentMethod = PaymentMethodType.UPI
            };

            _requestRepoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<PaymentRecord>()))
                .Callback<PaymentRecord>(p => savedPayment = p)
                .Returns(Task.CompletedTask);
            _requestRepoMock.Setup(r => r.UpdateAsync(req)).Returns(Task.CompletedTask);
            _paymentRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _service.ProcessPaymentAsync(dto);

         
            Assert.NotNull(savedPayment);
            Assert.Equal(PaymentMethodType.UPI, savedPayment!.PaymentMethod);
            Assert.Equal(req.ReimbursementRequestId, savedPayment.ReimbursementRequestId);
        }

        [Fact]
        public async Task ProcessPaymentAsync_SendsNotificationToUser()
        {
         
            var userId = Guid.NewGuid();
            var req = MakeRequest(userId: userId);
            Guid? notifiedUserId = null;

            var dto = new ProcessPaymentRequestDto
            {
                ReimbursementRequestId = req.ReimbursementRequestId,
                Amount = req.Amount,
                PaymentMethod = PaymentMethodType.BankTransfer
            };

            _requestRepoMock.Setup(r => r.GetByIdAsync(req.ReimbursementRequestId)).ReturnsAsync(req);
            _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<PaymentRecord>())).Returns(Task.CompletedTask);
            _requestRepoMock.Setup(r => r.UpdateAsync(req)).Returns(Task.CompletedTask);
            _paymentRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock
                .Setup(n => n.SendNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback<Guid, string>((uid, _) => notifiedUserId = uid)
                .Returns(Task.CompletedTask);

         
            await _service.ProcessPaymentAsync(dto);

            Assert.Equal(userId, notifiedUserId);
        }

        [Fact]
        public async Task GetAllPaymentsAsync_ReturnsAllPayments()
        {
 
            var payments = new List<PaymentRecord>
            {
                new PaymentRecord { PaymentRecordId = Guid.NewGuid(), ReimbursementRequestId = Guid.NewGuid(), AmountPaid = 1000m, PaymentMethod = PaymentMethodType.Cash, TransactionReference = "T1", PaymentDate = DateTime.UtcNow },
                new PaymentRecord { PaymentRecordId = Guid.NewGuid(), ReimbursementRequestId = Guid.NewGuid(), AmountPaid = 2000m, PaymentMethod = PaymentMethodType.UPI, TransactionReference = "T2", PaymentDate = DateTime.UtcNow }
            };

            _paymentRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(payments);


            var result = await _service.GetAllPaymentsAsync();

            Assert.Equal(2, result.Count());
        }


        [Fact]
        public async Task GetByRequestIdAsync_ExistingId_ReturnsPayment()
        {
     
            var requestId = Guid.NewGuid();
            var payment = new PaymentRecord
            {
                PaymentRecordId = Guid.NewGuid(),
                ReimbursementRequestId = requestId,
                AmountPaid = 5000m,
                PaymentMethod = PaymentMethodType.BankTransfer,
                TransactionReference = "TXN123",
                PaymentDate = DateTime.UtcNow
            };

            _paymentRepoMock.Setup(r => r.GetByRequestIdAsync(requestId)).ReturnsAsync(payment);

     
            var result = await _service.GetByRequestIdAsync(requestId);

            Assert.NotNull(result);
            Assert.Equal(requestId, result!.ReimbursementRequestId);
        }

        [Fact]
        public async Task GetByRequestIdAsync_NotFound_ReturnsNull()
        {
    
            _paymentRepoMock.Setup(r => r.GetByRequestIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((PaymentRecord?)null);

     
            var result = await _service.GetByRequestIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }
}
