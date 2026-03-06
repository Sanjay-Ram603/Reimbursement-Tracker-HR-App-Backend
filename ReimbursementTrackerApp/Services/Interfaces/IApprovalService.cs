using ReimbursementTrackerApp.DataTransferObjects.Approval;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IApprovalService
    {
        Task ProcessApprovalAsync(Guid approverUserId, ApprovalActionRequestDto request);
       
    }
}
