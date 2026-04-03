using ReimbursementTrackerApp.DataTransferObjects.Approval;
using ReimbursementTrackerApp.Models.Approval;
using ReimbursementTrackerApp.Models.Enumerations;
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

            
            reimbursement.Status = request.Status;

            var approval = new ApprovalHistory
            {
                ApprovalHistoryId = Guid.NewGuid(),
                ReimbursementRequestId = reimbursement.ReimbursementRequestId,
                ApproverUserId = approverUserId,
                ApprovalStage = ApprovalStageType.Manager,
                Status = request.Status,
                Comments = request.Comments,
                ActionDate = DateTime.UtcNow
            };

            await _approvalRepository.AddAsync(approval);
            await _requestRepository.UpdateAsync(reimbursement);
            await _approvalRepository.SaveChangesAsync();

            string message = reimbursement.Status switch
            {
                ReimbursementStatusType.ManagerApproved =>
                    "Your reimbursement has been approved.",
                ReimbursementStatusType.FinanceApproved =>
                    "Your reimbursement has been Finance Approved.",
                ReimbursementStatusType.Rejected =>
                    "Your reimbursement request has been Rejected.",
                ReimbursementStatusType.Paid =>
                    "Your reimbursement has been Paid successfully!",
                _ => "Your reimbursement status has been updated."
            };

            await _notificationService.SendNotificationAsync(reimbursement.UserId, message);
        }

        public async Task<IEnumerable<ApprovalHistoryResponseDto>> GetApprovalHistoryAsync(Guid requestId)
        {
            var histories = await _approvalRepository.GetByRequestIdAsync(requestId);

            return histories.Select(h => new ApprovalHistoryResponseDto
            {
                ReimbursementRequestId = h.ReimbursementRequestId,
                ApproverName = h.ApproverUser != null
                    ? $"{h.ApproverUser.FirstName} {h.ApproverUser.LastName}"
                    : "Unknown",
                Comments = h.Comments,
                Status = h.Status,
                ActionDate = h.ActionDate
            });
        }
    }
}