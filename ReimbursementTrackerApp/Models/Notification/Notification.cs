using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Notification
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public Identity.User? User { get; set; }
    }
}
