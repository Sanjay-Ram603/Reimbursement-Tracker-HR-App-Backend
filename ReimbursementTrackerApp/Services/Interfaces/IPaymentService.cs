using ReimbursementTrackerApp.DataTransferObjects.Payment;
using ReimbursementTrackerApp.Models.Payment;


namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IPaymentService
    {
        Task ProcessPaymentAsync(ProcessPaymentRequestDto request);

        Task<IEnumerable<PaymentRecord>> GetAllPaymentsAsync();
        Task<PaymentRecord> GetByRequestIdAsync(Guid requestId);

    }
}
