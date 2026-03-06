using ReimbursementTrackerApp.Models.Enumerations;

namespace ReimbursementTrackerApp.DataTransferObjects.Approval
{
    public class ApprovalActionRequestDto
    {
        public Guid ReimbursementRequestId { get; set; }
        public ReimbursementStatusType Status { get; set; }
        public string? Comments { get; set; }
    }
}
