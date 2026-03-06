
using ReimbursementTrackerApp.Models.Reimbursement;

namespace ReimbursementTrackerApp.Repositories.Interfaces
{
    public interface IReimbursementRequestRepository
    {
        Task<ReimbursementRequest?> GetByIdAsync(Guid requestId);
        Task<IEnumerable<ReimbursementRequest>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<ReimbursementRequest>> GetAllAsync();
        Task AddAsync(ReimbursementRequest request);
        Task UpdateAsync(ReimbursementRequest request);
        Task SaveChangesAsync();

        void Delete(ReimbursementRequest request);


    }
}
