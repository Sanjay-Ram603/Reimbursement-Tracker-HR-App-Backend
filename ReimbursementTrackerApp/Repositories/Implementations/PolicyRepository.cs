using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Models.Policy;


namespace ReimbursementTrackerApp.Repositories.Implementations
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly ReimbursementDbContext _context;

        public PolicyRepository(ReimbursementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PolicyRule>> GetAllActiveRulesAsync()
        {
            return await _context.PolicyRules
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task AddViolationAsync(PolicyViolation violation)
        {
            await _context.PolicyViolations.AddAsync(violation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
