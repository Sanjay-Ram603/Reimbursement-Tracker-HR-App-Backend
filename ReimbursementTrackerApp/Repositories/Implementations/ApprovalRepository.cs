using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.Models.Approval;
using ReimbursementTrackerApp.Repositories.Interfaces;

namespace ReimbursementTrackerApp.Repositories.Implementations
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly ReimbursementDbContext _context;

        public ApprovalRepository(ReimbursementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ApprovalHistory approvalHistory)
        {
            await _context.ApprovalHistories.AddAsync(approvalHistory);
        }

        public async Task<IEnumerable<ApprovalHistory>> GetByRequestIdAsync(Guid requestId)
        {
            return await _context.ApprovalHistories
                .Where(a => a.ReimbursementRequestId == requestId)
                .Include(a => a.ApproverUser)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApprovalHistory>> GetAllAsync()
        {
            return await _context.ApprovalHistories.ToListAsync();
        }



    }
}
