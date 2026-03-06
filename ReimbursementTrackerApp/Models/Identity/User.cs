using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Identity
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Role? Role { get; set; }

        public ICollection<Reimbursement.ReimbursementRequest>? ReimbursementRequests { get; set; }
    }
}
