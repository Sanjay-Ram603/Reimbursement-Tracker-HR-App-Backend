
using ReimbursementTrackerApp.Models.Payment;

namespace ReimbursementTrackerApp.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(PaymentRecord paymentRecord);
        Task<PaymentRecord?> GetByRequestIdAsync(Guid requestId);
        Task SaveChangesAsync();
    }
}
