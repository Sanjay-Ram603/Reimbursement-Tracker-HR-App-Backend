
using ReimbursementTrackerApp.DataTransferObjects.Report;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReimbursementRequestRepository _repository;

        public ReportService(IReimbursementRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReimbursementReportResponseDto>> GenerateReportAsync(
    DateTime fromDate,
    DateTime toDate,
    string? status,
    int startIndex,
    int pageSize)
        {
            var requests = await _repository.GetAllAsync();

            // 🔥 DATE FILTER
            var filtered = requests
                .Where(r => r.CreatedAt >= fromDate && r.CreatedAt <= toDate);

            // 🔥 STATUS FILTER
            if (!string.IsNullOrEmpty(status))
            {
                switch (status.ToLower())
                {
                    case "pending":
                        filtered = filtered.Where(r =>
                            r.Status == Models.Enumerations.ReimbursementStatusType.Submitted ||
                            r.Status == Models.Enumerations.ReimbursementStatusType.ManagerApproved);
                        break;

                    case "approved":
                        filtered = filtered.Where(r =>
                            r.Status == Models.Enumerations.ReimbursementStatusType.FinanceApproved ||
                            r.Status == Models.Enumerations.ReimbursementStatusType.Paid);
                        break;

                    case "rejected":
                        filtered = filtered.Where(r =>
                            r.Status == Models.Enumerations.ReimbursementStatusType.Rejected);
                        break;

                    default:
                        throw new Exception("Invalid status filter.");
                }
            }

            // 🔥 FINAL RESULT + PAGINATION
            return filtered
                .Select(r => new ReimbursementReportResponseDto
                {
                    ReimbursementRequestId = r.ReimbursementRequestId,
                    Amount = r.Amount,
                    CreatedAt = r.CreatedAt
                })
                .Skip(startIndex * pageSize)
                .Take(pageSize);
        }


    }
}
