using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Identity
{
    public class RefreshToken
    {
        [Key]
        public Guid RefreshTokenId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
