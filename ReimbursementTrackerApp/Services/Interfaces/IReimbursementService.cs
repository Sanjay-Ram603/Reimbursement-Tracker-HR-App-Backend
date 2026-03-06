using ReimbursementTrackerApp.DataTransferObjects.Reimbursement;

namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface IReimbursementService
    {
        Task<Guid> CreateRequestAsync(Guid userId, CreateReimbursementRequestDto request);
        Task<IEnumerable<ReimbursementRequestResponseDto>> GetUserRequestsAsync(Guid userId);
        Task UpdateStatusAsync(Guid requestId, UpdateReimbursementStatusRequestDto request);
        Task<bool> DeleteReimbursementRequest(Guid id);

    }
}
