using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Identity
{
    public class Role
    {
        [Key]
        public Guid RoleId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<User>? Users { get; set; }
    }
}
