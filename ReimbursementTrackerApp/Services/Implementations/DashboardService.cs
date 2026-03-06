using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.DataTransferObjects.DashboardSummary;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{

    public class DashboardService : IDashboardService
    {
        private readonly ReimbursementDbContext _context;

        public DashboardService(ReimbursementDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetMyDashboardAsync(Guid userId)
        {
            var requests = await _context.ReimbursementRequests
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return new DashboardSummaryDto
            {
                TotalRequests = requests.Count,
                ApprovedRequests = requests.Count(x => x.Status == ReimbursementStatusType.ManagerApproved || x.Status == ReimbursementStatusType.FinanceApproved),
                RejectedRequests = requests.Count(x => x.Status == ReimbursementStatusType.Rejected),
                PendingRequests = requests.Count(x => x.Status == ReimbursementStatusType.Submitted),
                TotalAmount = requests.Sum(x => x.Amount)
            };
        }
    }


}
