using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;

using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Models.Identity;

namespace ReimbursementSystem.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ReimbursementDbContext _context;

        public RoleRepository(ReimbursementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(Guid roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task<Role?> GetByRoleNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
