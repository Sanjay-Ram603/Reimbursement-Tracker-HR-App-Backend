using ReimbursementTrackerApp.Models.Enumerations;

namespace ReimbursementTrackerApp.DataTransferObjects.Reimbursement
{
    public class ReimbursementRequestResponseDto
    {
        public Guid ReimbursementRequestId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public ReimbursementStatusType Status { get; set; }
    }
}
