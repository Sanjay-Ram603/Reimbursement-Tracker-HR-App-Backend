using ReimbursementTrackerApp.Models.Enumerations;

namespace ReimbursementTrackerApp.DataTransferObjects.Payment
{
    public class ProcessPaymentRequestDto
    {
        public Guid ReimbursementRequestId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
    }
}
