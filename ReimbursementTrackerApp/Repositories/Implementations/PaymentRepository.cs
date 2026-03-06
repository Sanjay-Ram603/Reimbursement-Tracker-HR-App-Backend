using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.Models.Payment;
using ReimbursementTrackerApp.Repositories.Interfaces;

namespace ReimbursementTrackerApp.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ReimbursementDbContext _context;

        public PaymentRepository(ReimbursementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PaymentRecord paymentRecord)
        {
            await _context.PaymentRecords.AddAsync(paymentRecord);
        }

        public async Task<PaymentRecord?> GetByRequestIdAsync(Guid requestId)
        {
            return await _context.PaymentRecords
                .FirstOrDefaultAsync(p => p.ReimbursementRequestId == requestId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
