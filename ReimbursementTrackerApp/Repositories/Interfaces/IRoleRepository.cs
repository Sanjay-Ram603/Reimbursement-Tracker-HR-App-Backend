
using ReimbursementTrackerApp.Models.Identity;

namespace ReimbursementTrackerApp.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid roleId);
        Task<Role?> GetByRoleNameAsync(string roleName);
        Task<IEnumerable<Role>> GetAllAsync();
        Task AddAsync(Role role);
        Task SaveChangesAsync();
    }
}
