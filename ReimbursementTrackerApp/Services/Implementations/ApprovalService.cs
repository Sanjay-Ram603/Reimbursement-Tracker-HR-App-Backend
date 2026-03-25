
using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.DataTransferObjects.Approval;
using ReimbursementTrackerApp.DataTransferObjects.PendingApproval;
using ReimbursementTrackerApp.Models.Approval;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Implementations;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{

    public class ApprovalService : IApprovalService
    {
        // ✅ 1. Fields 
        private readonly IApprovalRepository _approvalRepository;
        private readonly IReimbursementRequestRepository _requestRepository;
        private readonly INotificationService _notificationService;

        // ✅ 2. CONSTRUCTOR 
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

            // ✅ STAGE VALIDATION
            if (request.Status == ReimbursementStatusType.ManagerApproved &&
                reimbursement.Status != ReimbursementStatusType.Submitted)
                throw new Exception("Only Submitted requests can be Manager Approved.");

            if (request.Status == ReimbursementStatusType.FinanceApproved &&
                reimbursement.Status != ReimbursementStatusType.ManagerApproved)
                throw new Exception("Only Manager Approved requests can be Finance Approved.");

            if (request.Status == ReimbursementStatusType.Rejected &&
                reimbursement.Status != ReimbursementStatusType.Submitted &&
                reimbursement.Status != ReimbursementStatusType.ManagerApproved)
                throw new Exception("Only Submitted or Manager Approved requests can be Rejected.");

            reimbursement.Status = request.Status;

            var approval = new ApprovalHistory
            {
                ApprovalHistoryId = Guid.NewGuid(),
                ReimbursementRequestId = reimbursement.ReimbursementRequestId,
                ApproverUserId = approverUserId,
                ApprovalStage = request.Status == ReimbursementStatusType.ManagerApproved
                    ? ApprovalStageType.Manager
                    : ApprovalStageType.Finance,
                Status = request.Status,
                Comments = request.Comments,
                ActionDate = DateTime.UtcNow
            };

            await _approvalRepository.AddAsync(approval);
            await _requestRepository.UpdateAsync(reimbursement);
            await _approvalRepository.SaveChangesAsync();

            string message = reimbursement.Status switch
            {
                ReimbursementStatusType.ManagerApproved => "Your reimbursement has been approved by Admin. Pending Finance approval.",
                ReimbursementStatusType.FinanceApproved => "Your reimbursement has been approved by Finance. Payment will be processed soon.",
                ReimbursementStatusType.Rejected => "Your reimbursement request has been Rejected.",
                _ => "Your reimbursement status has been updated."
            };

            await _notificationService.SendNotificationAsync(reimbursement.UserId, message);
        }

        public async Task<IEnumerable<ApprovalHistory>> GetAllApprovalsAsync()
        {
            return await _approvalRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ApprovalHistory>> GetByRequestIdAsync(Guid requestId)
        {
            return await _approvalRepository.GetByRequestIdAsync(requestId);
        }




    }

}
