namespace ReimbursementTrackerApp.DataTransferObjects.ExpenseCategory
{
    public class ExpenseCategoryDto
    {
        public Guid ExpenseCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
