using ReimbursementTrackerApp.Models.Reimbursement;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IPolicyService
    {
        Task ValidatePoliciesAsync(ReimbursementRequest request);
    }
}
