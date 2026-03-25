using ReimbursementTrackerApp.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Reimbursement
{
    public class ReimbursementRequest
    {
        [Key]
        public Guid ReimbursementRequestId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ExpenseCategoryId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ReimbursementStatusType Status { get; set; }

        [Required]
        public string? AttachmentPath { get; set; }


        [Required]
        public DateTime ExpenseDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Identity.User? User { get; set; }

        public ExpenseCategory? ExpenseCategory { get; set; }

        public ICollection<ReimbursementAttachment>? Attachments { get; set; }

        public ICollection<ReimbursementStatusHistory>? StatusHistories { get; set; }

    }
}
