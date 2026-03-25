using ReimbursementTrackerApp.Models.Reimbursement;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IExpenseCategoryService
    {

        Task<IEnumerable<ExpenseCategory>> GetAllAsync();
        Task<ExpenseCategory?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(string categoryName);
        Task UpdateAsync(Guid id, string categoryName);
        Task<bool> DeleteAsync(Guid id);

    }
}
