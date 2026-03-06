using System.ComponentModel.DataAnnotations;

 namespace ReimbursementTrackerApp.Models.Audit
    {
        public class AuditLog
        {
            [Key]
            public Guid AuditLogId { get; set; }

            [Required]
            public string EntityName { get; set; } = string.Empty;

            [Required]
            public string ActionType { get; set; } = string.Empty;

            [Required]
            public Guid PerformedByUserId { get; set; }

            public string? Changes { get; set; }

            public DateTime PerformedAt { get; set; }
        }
    }
