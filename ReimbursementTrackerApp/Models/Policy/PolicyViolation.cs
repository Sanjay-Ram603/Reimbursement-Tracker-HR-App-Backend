using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Policy
{
    public class PolicyViolation
    {
        [Key]
        public Guid PolicyViolationId { get; set; }

        [Required]
        public Guid ReimbursementRequestId { get; set; }

        [Required]
        public Guid PolicyRuleId { get; set; }

        public string? ViolationMessage { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public Reimbursement.ReimbursementRequest? ReimbursementRequest { get; set; }
        public PolicyRule? PolicyRule { get; set; }
    }
}
