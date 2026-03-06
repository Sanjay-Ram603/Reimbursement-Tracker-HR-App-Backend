
using ReimbursementTrackerApp.Models.Policy;

namespace ReimbursementTrackerApp.Repositories.Interfaces
{
    public interface IPolicyRepository
    {
        Task<IEnumerable<PolicyRule>> GetAllActiveRulesAsync();
        Task AddViolationAsync(PolicyViolation violation);
        Task SaveChangesAsync();
    }
}
