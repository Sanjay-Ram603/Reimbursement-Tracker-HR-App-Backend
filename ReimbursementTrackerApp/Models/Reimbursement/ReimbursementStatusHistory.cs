using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReimbursementTrackerApp.Models.Reimbursement
{
    public class ReimbursementStatusHistory
    {
        [Key]
        public Guid ReimbursementStatusHistoryId { get; set; }

        [Required]
        public Guid ReimbursementRequestId { get; set; }

        [Required]
        public ReimbursementStatusType OldStatus { get; set; }

        [Required]
        public ReimbursementStatusType NewStatus { get; set; }

        [Required]
        public Guid ChangedByUserId { get; set; }

        public string? Comments { get; set; }

        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(ReimbursementRequestId))]
        public ReimbursementRequest? ReimbursementRequest { get; set; }

        [ForeignKey(nameof(ChangedByUserId))]
        public User? ChangedByUser { get; set; }
    }
}
