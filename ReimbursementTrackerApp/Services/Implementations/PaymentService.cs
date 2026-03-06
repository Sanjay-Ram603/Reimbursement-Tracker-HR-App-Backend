
using ReimbursementTrackerApp.DataTransferObjects.Payment;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Payment;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReimbursementRequestRepository _requestRepository;
        private readonly INotificationService _notificationService;

        public PaymentService(
    IPaymentRepository paymentRepository,
    IReimbursementRequestRepository requestRepository,
    INotificationService notificationService)
        {
            _paymentRepository = paymentRepository;
            _requestRepository = requestRepository;
            _notificationService = notificationService;
        }

        public async Task ProcessPaymentAsync(ProcessPaymentRequestDto request)
        {
            var reimbursement = await _requestRepository
                .GetByIdAsync(request.ReimbursementRequestId);

            if (reimbursement == null)
                throw new Exception("Reimbursement request not found.");

            // 1️⃣ Create payment record
            var payment = new PaymentRecord
            {
                PaymentRecordId = Guid.NewGuid(),
                ReimbursementRequestId = request.ReimbursementRequestId,
                AmountPaid = request.Amount,
                PaymentMethod = request.PaymentMethod,
                PaymentDate = DateTime.UtcNow,
                TransactionReference = Guid.NewGuid().ToString()
            };

            await _paymentRepository.AddAsync(payment);

            // 2️⃣ Update reimbursement status
            reimbursement.Status = ReimbursementStatusType.Paid;
            await _requestRepository.UpdateAsync(reimbursement);

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();


            // Save everything
            //await _paymentRepository.SaveChangesAsync();

            // 3️⃣ Create notification
            await _notificationService.SendNotificationAsync(
                reimbursement.UserId,
                "Your reimbursement amount has been successfully paid by Finance."
            );
        }

    }
}
