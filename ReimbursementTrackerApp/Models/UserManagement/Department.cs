using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.UserManagement
{
    public class Department
    {
        [Key]
        public Guid DepartmentId { get; set; }

        [Required]
        public string DepartmentName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
