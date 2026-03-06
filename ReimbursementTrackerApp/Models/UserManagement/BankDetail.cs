using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.UserManagement
{
    public class BankDetail
    {
        [Key]
        public Guid BankDetailId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string BankName { get; set; } = string.Empty;

        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        public string IFSCCode { get; set; } = string.Empty;

        // Navigation
        public Identity.User? User { get; set; }
    }
}
