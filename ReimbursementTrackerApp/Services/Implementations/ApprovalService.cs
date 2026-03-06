
using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.DataTransferObjects.Approval;
using ReimbursementTrackerApp.DataTransferObjects.PendingApproval;
using ReimbursementTrackerApp.Models.Approval;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class ApprovalService : IApprovalService
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly IReimbursementRequestRepository _requestRepository;
        private readonly INotificationService _notificationService;



     public ApprovalService(
     IApprovalRepository approvalRepository,
     IReimbursementRequestRepository requestRepository,
     INotificationService notificationService)
        {
            _approvalRepository = approvalRepository;
            _requestRepository = requestRepository;
            _notificationService = notificationService;
        }


    



        public async Task ProcessApprovalAsync(Guid approverUserId, ApprovalActionRequestDto request)
        {
            var reimbursement = await _requestRepository.GetByIdAsync(request.ReimbursementRequestId);
            if (reimbursement == null)
                throw new Exception("Request not found.");

            // Update status
            reimbursement.Status = request.Status;

            var approval = new ApprovalHistory
            {
                ApprovalHistoryId = Guid.NewGuid(),
                ReimbursementRequestId = reimbursement.ReimbursementRequestId,
                ApproverUserId = approverUserId,
                Status = request.Status,
                ActionDate = DateTime.UtcNow
            };

            await _approvalRepository.AddAsync(approval);
            await _requestRepository.UpdateAsync(reimbursement);

            await _approvalRepository.SaveChangesAsync();

            // 🔥 ADD THIS PART (Notification)
            string message = reimbursement.Status switch
            {
                ReimbursementStatusType.ManagerApproved => "Your reimbursement has been Manager Approved.",
                ReimbursementStatusType.FinanceApproved => "Your reimbursement has been Finance Approved.",
                ReimbursementStatusType.Rejected => "Your reimbursement has been Rejected.",
                _ => "Your reimbursement status has been updated."
            };


            await _notificationService.SendNotificationAsync(
                reimbursement.UserId,
                message
            );
        }

    }
}
