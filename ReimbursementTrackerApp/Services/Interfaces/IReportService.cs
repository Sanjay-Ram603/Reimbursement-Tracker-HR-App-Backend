using ReimbursementTrackerApp.DataTransferObjects.Report;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<ReimbursementReportResponseDto>> GenerateReportAsync(
            DateTime fromDate,
            DateTime toDate,
            string? status,
            string? role,
            int startIndex,
            int pageSize);

        Task<int> GetTotalCountAsync(
            DateTime fromDate,
            DateTime toDate,
            string? status,
            string? role);
    }
}