using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.DataTransferObjects.Authentication
{
    public class RegisterUserRequestDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public Guid RoleId { get; set; }
    }
}