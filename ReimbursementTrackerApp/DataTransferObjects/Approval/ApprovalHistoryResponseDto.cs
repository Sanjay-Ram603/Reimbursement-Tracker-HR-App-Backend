using ReimbursementTrackerApp.Models.Enumerations;

namespace ReimbursementTrackerApp.DataTransferObjects.Approval
{
    public class ApprovalHistoryResponseDto
    {

        public Guid ReimbursementRequestId { get; set; }
        public string ApproverName { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public ReimbursementStatusType Status { get; set; }
        public DateTime ActionDate { get; set; }

    }
}
