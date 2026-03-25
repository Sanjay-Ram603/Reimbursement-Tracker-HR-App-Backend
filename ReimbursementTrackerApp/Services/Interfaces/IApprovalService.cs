using ReimbursementTrackerApp.DataTransferObjects.Approval;
using ReimbursementTrackerApp.Models.Approval;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IApprovalService
    {
        Task ProcessApprovalAsync(Guid approverUserId, ApprovalActionRequestDto request);

        Task<IEnumerable<ApprovalHistory>> GetAllApprovalsAsync();
        Task<IEnumerable<ApprovalHistory>> GetByRequestIdAsync(Guid requestId);



    }

}
