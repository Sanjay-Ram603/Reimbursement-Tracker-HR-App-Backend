
using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.DataTransferObjects.Reimbursement;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class ReimbursementService : IReimbursementService
    {
        private readonly IReimbursementRequestRepository _repository;

        public ReimbursementService(IReimbursementRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateRequestAsync(Guid userId, CreateReimbursementRequestDto request)
        {
            var entity = new ReimbursementRequest
            {
                ReimbursementRequestId = Guid.NewGuid(),
                UserId = userId,
                ExpenseCategoryId = request.ExpenseCategoryId,
                Amount = request.Amount,
                Description = request.Description,
                ExpenseDate = request.ExpenseDate,
                CreatedAt = DateTime.UtcNow,
                Status = request.Status
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ReimbursementRequestId;
        }

        public async Task<IEnumerable<ReimbursementRequestResponseDto>> GetUserRequestsAsync(Guid userId)
        {
            var requests = await _repository.GetByUserIdAsync(userId);

            return requests.Select(r => new ReimbursementRequestResponseDto
            {
                ReimbursementRequestId = r.ReimbursementRequestId,
                Amount = r.Amount,
                Description = r.Description,
                Status = r.Status
            });
        }

        public async Task UpdateStatusAsync(Guid requestId, UpdateReimbursementStatusRequestDto request)
        {
            var entity = await _repository.GetByIdAsync(requestId);
            if (entity == null)
                throw new Exception("Request not found.");

            entity.Status = request.Status ?? entity.Status;
            entity.Description = request.Description ?? entity.Description;
            entity.Amount = request.Amount ?? entity.Amount;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteReimbursementRequest(Guid id)
        {
            var request = await _repository.GetByIdAsync(id);

            if (request == null)
                return false;

            _repository.Delete(request);
            await _repository.SaveChangesAsync();

            return true;
        }

    }
}
