using ReimbursementTrackerApp.DataTransferObjects.Report;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<ReimbursementReportResponseDto>> GenerateReportAsync(DateTime fromDate, DateTime toDate, int startIndex, int pageSize);
    }
}
