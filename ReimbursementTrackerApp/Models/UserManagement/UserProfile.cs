using ReimbursementTrackerApp.Models.UserManagement;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace ReimbursementTrackerApp.Models.UserManagement
{
    public class UserProfile
    {
        [Key]
        public Guid UserProfileId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        public Guid? DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Identity.User? User { get; set; }

        public Department? Department { get; set; }
    }
}
