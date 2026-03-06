
using ReimbursementTrackerApp.Models.Reimbursement;

namespace ReimbursementTrackerApp.Repositories.Interfaces
{
    public interface IExpenseCategoryRepository
    {
        Task<IEnumerable<ExpenseCategory>> GetAllAsync();
        Task<ExpenseCategory?> GetByIdAsync(Guid categoryId);
        Task AddAsync(ExpenseCategory category);
        Task SaveChangesAsync();
    }
}
