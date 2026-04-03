using ReimbursementTrackerApp.DataTransferObjects.Report;
using ReimbursementTrackerApp.Models.Enumerations;
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
            string? role,
            int startIndex,
            int pageSize)
        {
            var requests = await _repository.GetAllAsync();

            // DATE FILTER
            var filtered = requests
                .Where(r => r.CreatedAt >= fromDate && r.CreatedAt <= toDate)
                .Where(r => r.User != null);

            // ROLE + STATUS FILTER
            if (!string.IsNullOrEmpty(role) && role.ToLower() == "finance")
            {
                // FINANCE ROLE FILTERS
                if (string.IsNullOrEmpty(status) || status.ToLower() == "all")
                {
                    filtered = filtered.Where(r =>
                        r.Status == ReimbursementStatusType.ManagerApproved ||
                        r.Status == ReimbursementStatusType.FinanceApproved ||
                        r.Status == ReimbursementStatusType.Paid ||
                        r.Status == ReimbursementStatusType.Rejected);
                }
                else
                {
                    switch (status.ToLower())
                    {
                        case "financepending":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.ManagerApproved);
                            break;

                        case "financeapproved":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.FinanceApproved);
                            break;

                        case "financepaid":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Paid);
                            break;

                        case "financerejected":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Rejected);
                            break;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(role) && role.ToLower() == "head")
            {
                // HEAD - Only Manager & Finance role users claims
                filtered = filtered.Where(r =>
                    r.User != null &&
                    (r.User.Role!.RoleName == "Manager" ||
                     r.User.Role!.RoleName == "Finance"));

                // Then apply status filter
                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
                {
                    switch (status.ToLower())
                    {
                        case "headpending":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Submitted);
                            break;
                        case "headapproved":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.ManagerApproved);
                            break;
                        case "headpaid":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Paid);
                            break;
                        case "headrejected":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Rejected);
                            break;
                    }
                }
            }
            else
            {
                // MANAGER ROLE FILTERS
                if (string.IsNullOrEmpty(status) || status.ToLower() == "all")
                {
                    filtered = filtered.Where(r =>
                        r.Status == ReimbursementStatusType.Submitted ||
                        r.Status == ReimbursementStatusType.ManagerApproved ||
                        r.Status == ReimbursementStatusType.Rejected);
                }
                else
                {
                    switch (status.ToLower())
                    {
                        case "pending":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Submitted);
                            break;

                        case "approved":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.ManagerApproved);
                            break;

                        case "rejected":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Rejected);
                            break;
                    }
                }
            }

            // PAGINATION
            return filtered
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReimbursementReportResponseDto
                {
                    ReimbursementRequestId = r.ReimbursementRequestId,
                    Amount = r.Amount,
                    CreatedAt = r.CreatedAt,
                    Description = r.Description,
                    EmployeeName = r.User != null
                        ? $"{r.User.FirstName} {r.User.LastName}"
                        : "Unknown",
                    EmployeeEmail = r.User?.Email ?? string.Empty,
                    Status = r.Status.ToString()
                })
                .Skip(startIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<int> GetTotalCountAsync(
            DateTime fromDate,
            DateTime toDate,
            string? status,
            string? role)
        {
            var requests = await _repository.GetAllAsync();

            var filtered = requests
                .Where(r => r.CreatedAt >= fromDate && r.CreatedAt <= toDate)
                .Where(r => r.User != null);

            if (!string.IsNullOrEmpty(role) && role.ToLower() == "finance")
            {
                if (string.IsNullOrEmpty(status) || status.ToLower() == "all")
                {
                    filtered = filtered.Where(r =>
                        r.Status == ReimbursementStatusType.ManagerApproved ||
                        r.Status == ReimbursementStatusType.FinanceApproved ||
                        r.Status == ReimbursementStatusType.Paid ||
                        r.Status == ReimbursementStatusType.Rejected);
                }
                else
                {
                    switch (status.ToLower())
                    {
                        case "financepending":
                            filtered = filtered.Where(r => r.Status == ReimbursementStatusType.ManagerApproved);
                            break;

                        case "financeapproved":
                            filtered = filtered.Where(r => r.Status == ReimbursementStatusType.FinanceApproved);
                            break;

                        case "financepaid":
                            filtered = filtered.Where(r => r.Status == ReimbursementStatusType.Paid);
                            break;

                        case "financerejected":
                            filtered = filtered.Where(r => r.Status == ReimbursementStatusType.Rejected);
                            break;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(role) && role.ToLower() == "head")
            {
                // HEAD - Only Manager & Finance role users claims
                filtered = filtered.Where(r =>
                    r.User != null &&
                    (r.User.Role!.RoleName == "Manager" ||
                     r.User.Role!.RoleName == "Finance"));

                // Then apply status filter
                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
                {
                    switch (status.ToLower())
                    {
                        case "headpending":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Submitted);
                            break;
                        case "headapproved":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.ManagerApproved);
                            break;
                        case "headpaid":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Paid);
                            break;
                        case "headrejected":
                            filtered = filtered.Where(r =>
                                r.Status == ReimbursementStatusType.Rejected);
                            break;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(status) || status.ToLower() == "all")
                {
                    filtered = filtered.Where(r =>
                        r.Status == ReimbursementStatusType.Submitted ||
                        r.Status == ReimbursementStatusType.ManagerApproved ||
                        r.Status == ReimbursementStatusType.Rejected);
                }
                else
                {
                    switch (status.ToLower())
                    {
                        case "pending":
                            filtered = filtered.Where(r => r.Status == ReimbursementStatusType.Submitted);
                            break;

                        case "approved":
                            filtered = filtered.Where(r => r.Status == ReimbursementStatusType.ManagerApproved);
                            break;

                        case "rejected":
                            filtered = filtered.Where(r => r.Status == ReimbursementStatusType.Rejected);
                            break;
                    }
                }
            }

            return filtered.Count();
        }
    }
}