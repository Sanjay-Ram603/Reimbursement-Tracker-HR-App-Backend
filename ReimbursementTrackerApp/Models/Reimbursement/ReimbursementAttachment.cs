using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReimbursementTrackerApp.Models.Reimbursement
{
    public class ReimbursementAttachment
    {
        [Key]
        public Guid ReimbursementAttachmentId { get; set; }

        [Required]
        public Guid ReimbursementRequestId { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey(nameof(ReimbursementRequestId))]
        public ReimbursementRequest? ReimbursementRequest { get; set; }
    }
}
