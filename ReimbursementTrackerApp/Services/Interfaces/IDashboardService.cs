using ReimbursementTrackerApp.DataTransferObjects.DashboardSummary;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetMyDashboardAsync(Guid userId);
    }
}
