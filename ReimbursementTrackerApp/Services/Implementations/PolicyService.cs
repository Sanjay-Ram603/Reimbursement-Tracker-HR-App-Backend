
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;

        public PolicyService(IPolicyRepository policyRepository)
        {
            _policyRepository = policyRepository;
        }

        public async Task ValidatePoliciesAsync(ReimbursementRequest request)
        {
            var rules = await _policyRepository.GetAllActiveRulesAsync();

            foreach (var rule in rules)
            {
                if (rule.MaximumAmount.HasValue &&
                    request.Amount > rule.MaximumAmount.Value)
                {
                    throw new Exception("Policy violation: Amount exceeds limit.");
                }
            }
        }
    }
}