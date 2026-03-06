using ReimbursementTrackerApp.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Payment
{
    public class PaymentRecord
    {
        [Key]
        public Guid PaymentRecordId { get; set; }

        [Required]
        public Guid ReimbursementRequestId { get; set; }

        [Required]
        public decimal AmountPaid { get; set; }

        [Required]
        public PaymentMethodType PaymentMethod { get; set; }

        [Required]
        public string? TransactionReference { get; set; }

        public DateTime PaymentDate { get; set; }

        // Navigation
        public Reimbursement.ReimbursementRequest? ReimbursementRequest { get; set; }
    }
}
