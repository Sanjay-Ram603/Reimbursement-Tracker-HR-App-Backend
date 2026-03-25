using ReimbursementTrackerApp.Models.Enumerations;
using Microsoft.AspNetCore.Http;

namespace ReimbursementTrackerApp.DataTransferObjects.Reimbursement
{
    public class CreateReimbursementRequestDto
    {
        public Guid ExpenseCategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public IFormFile? Attachment { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}
