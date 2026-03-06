
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

        public async Task<IEnumerable<ReimbursementReportResponseDto>> GenerateReportAsync(DateTime fromDate, DateTime toDate, int startIndex, int pageSize)
        {
            var requests = await _repository.GetAllAsync();

            return requests
                .Where(r => r.CreatedAt >= fromDate && r.CreatedAt <= toDate)
                .Select(r => new ReimbursementReportResponseDto
                {
                    ReimbursementRequestId = r.ReimbursementRequestId,
                    Amount = r.Amount,
                    CreatedAt = r.CreatedAt
                }).Skip(startIndex * pageSize).Take(pageSize);
        }
    }
}
