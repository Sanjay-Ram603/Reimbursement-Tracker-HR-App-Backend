using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Models.Reimbursement;

namespace ReimbursementSystem.Repositories.Implementations
{
    public class ExpenseCategoryRepository : IExpenseCategoryRepository
    {
        private readonly ReimbursementDbContext _context;

        public ExpenseCategoryRepository(ReimbursementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExpenseCategory category)
        {
            await _context.ExpenseCategories.AddAsync(category);
        }

        public async Task<IEnumerable<ExpenseCategory>> GetAllAsync()
        {
            return await _context.ExpenseCategories.ToListAsync();
        }

        public async Task<ExpenseCategory?> GetByIdAsync(Guid categoryId)
        {
            return await _context.ExpenseCategories.FindAsync(categoryId);
        }

        // ✅ ADD THIS
        public Task UpdateAsync(ExpenseCategory category)
        {
            _context.ExpenseCategories.Update(category);
            return Task.CompletedTask;
        }

        // ✅ ADD THIS
        public Task DeleteAsync(ExpenseCategory category)
        {
            _context.ExpenseCategories.Remove(category);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
