using ReimbursementTrackerApp.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Approval
{
    public class ApprovalHistory
    {
        [Key]
        public Guid ApprovalHistoryId { get; set; }

        [Required]
        public Guid ReimbursementRequestId { get; set; }

        [Required]
        public Guid ApproverUserId { get; set; }

        [Required]
        public ApprovalStageType ApprovalStage { get; set; }

        [Required]
        public ReimbursementStatusType Status { get; set; }

        public string? Comments { get; set; }

        public DateTime ActionDate { get; set; }

       
        // Navigation
        
        public Reimbursement.ReimbursementRequest? ReimbursementRequest { get; set; }
        public Identity.User? ApproverUser { get; set; }
    }
}
