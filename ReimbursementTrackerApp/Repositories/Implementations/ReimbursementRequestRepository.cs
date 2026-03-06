using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;

namespace ReimbursementTrackerApp.Repositories.Implementations
{
    public class ReimbursementRequestRepository : IReimbursementRequestRepository
    {
        private readonly ReimbursementDbContext _context;

        public ReimbursementRequestRepository(ReimbursementDbContext context)
        {
            _context = context;
        }

        

        public async Task AddAsync(ReimbursementRequest request)
        {
            await _context.ReimbursementRequests.AddAsync(request);
        }

        public async Task<IEnumerable<ReimbursementRequest>> GetAllAsync()
        {
            return await _context.ReimbursementRequests
                .Include(r => r.ExpenseCategory)
                .ToListAsync();
        }

        public async Task<ReimbursementRequest?> GetByIdAsync(Guid requestId)
        {
            return await _context.ReimbursementRequests
                .Include(r => r.ExpenseCategory)
                .FirstOrDefaultAsync(r => r.ReimbursementRequestId == requestId);
        }

        public async Task<IEnumerable<ReimbursementRequest>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ReimbursementRequests
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(ReimbursementRequest request)
        {
            _context.ReimbursementRequests.Update(request);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Delete(ReimbursementRequest request)
        {
            _context.ReimbursementRequests.Remove(request);
        }

    }
}
