
using ReimbursementTrackerApp.Models.Approval;

namespace ReimbursementTrackerApp.Repositories.Interfaces
{
    public interface IApprovalRepository
    {
        Task AddAsync(ApprovalHistory approvalHistory);
        Task<IEnumerable<ApprovalHistory>> GetByRequestIdAsync(Guid requestId);
        Task SaveChangesAsync();
        Task<IEnumerable<ApprovalHistory>> GetAllAsync();
       


    }
}
