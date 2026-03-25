using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class ExpenseCategoryService : IExpenseCategoryService

    {

        private readonly IExpenseCategoryRepository _repository;

        public ExpenseCategoryService(IExpenseCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ExpenseCategory>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ExpenseCategory?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Guid> CreateAsync(string categoryName)
        {
            var entity = new ExpenseCategory
            {
                ExpenseCategoryId = Guid.NewGuid(),
                CategoryName = categoryName
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ExpenseCategoryId;
        }

        public async Task UpdateAsync(Guid id, string categoryName)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Category not found");

            entity.CategoryName = categoryName;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return false;

            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();

            return true;
        }

    }
}
