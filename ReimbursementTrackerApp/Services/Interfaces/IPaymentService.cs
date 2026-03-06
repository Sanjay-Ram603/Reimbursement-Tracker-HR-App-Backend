using ReimbursementTrackerApp.DataTransferObjects.Payment;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IPaymentService
    {
        Task ProcessPaymentAsync(ProcessPaymentRequestDto request);
    }
}
