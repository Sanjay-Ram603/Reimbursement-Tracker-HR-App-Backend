using ReimbursementTrackerApp.Models.Enumerations;

namespace ReimbursementTrackerApp.DataTransferObjects.Reimbursement
{
    public class CreateReimbursementRequestDto
    {
        public Guid ExpenseCategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public ReimbursementStatusType Status { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}
