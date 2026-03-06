using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Reimbursement
{
    public class ExpenseCategory
    {
        [Key]
        public Guid ExpenseCategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }



        public ICollection<ReimbursementRequest>? ReimbursementRequests { get; set; }
       
    }
}
