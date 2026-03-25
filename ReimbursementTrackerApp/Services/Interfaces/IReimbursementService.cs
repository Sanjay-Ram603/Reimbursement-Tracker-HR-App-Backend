using ReimbursementTrackerApp.DataTransferObjects.Reimbursement;
using ReimbursementTrackerApp.Models.Enumerations;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IReimbursementService
    {
        Task<Guid> CreateRequestAsync(Guid userId, CreateReimbursementRequestDto request);
        Task<IEnumerable<ReimbursementRequestResponseDto>> GetUserRequestsAsync(Guid userId);
        Task<IEnumerable<ReimbursementRequestResponseDto>> GetAllRequestsAsync();
        Task UpdateRequestAsync(Guid requestId, UpdateReimbursementStatusRequestDto request);
        Task UpdateStatusAsync(Guid requestId, ReimbursementStatusType newStatus);
        Task<ReimbursementRequestResponseDto?> GetByIdAsync(Guid id);
        Task<bool> DeleteReimbursementRequest(Guid id);
    }
}