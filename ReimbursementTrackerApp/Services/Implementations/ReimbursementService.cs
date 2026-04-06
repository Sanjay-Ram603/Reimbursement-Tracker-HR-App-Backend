using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.DataTransferObjects.Reimbursement;
using ReimbursementTrackerApp.Models.Enumerations;
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
            string? filePath = null;

            if (request.Attachment != null)
            {
                
                if (request.Attachment.Length > 5 * 1024 * 1024)
                    throw new Exception("File size exceeds 5MB");

               
                if (request.Attachment.Length == 0)
                    throw new Exception("Empty file is not allowed");


               
                var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var extension = Path.GetExtension(request.Attachment.FileName).ToLower();

                if (!allowedTypes.Contains(extension))
                    throw new Exception("Invalid file type. Only JPG, PNG, PDF allowed.");

                
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid() + extension;
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.Attachment.CopyToAsync(stream);
                }

                filePath = "/uploads/" + fileName;
            }


            var entity = new ReimbursementRequest
            {
                ReimbursementRequestId = Guid.NewGuid(),
                UserId = userId,
                ExpenseCategoryId = request.ExpenseCategoryId,
                Amount = request.Amount,
                Description = request.Description,
                ExpenseDate = request.ExpenseDate,
                CreatedAt = DateTime.UtcNow,

              
                Status = ReimbursementStatusType.Submitted,

              
                AttachmentPath = filePath
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
                Status = r.Status,
                AttachmentPath = r.AttachmentPath,
                CreatedAt = r.CreatedAt,
                EmployeeName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : "Unknown",
                EmployeeEmail = r.User?.Email ?? string.Empty,
                EmployeeRole = r.User?.Role?.RoleName ?? string.Empty
            });
        }


      
        public async Task UpdateRequestAsync(Guid requestId, UpdateReimbursementStatusRequestDto request)
        {
            var entity = await _repository.GetByIdAsync(requestId);

            if (entity == null)
                throw new Exception("Request not found.");

            if (entity.Status != ReimbursementStatusType.Submitted)
                throw new Exception("Only submitted requests can be edited.");

            entity.Amount = (decimal)request.Amount;
            entity.Description = request.Description;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }

       
        public async Task UpdateStatusAsync(Guid requestId, ReimbursementStatusType newStatus)
        {
            var entity = await _repository.GetByIdAsync(requestId);

            if (entity == null)
                throw new Exception("Request not found.");

            if (entity.Status == ReimbursementStatusType.Paid)
                throw new Exception("Paid requests cannot be modified.");

            entity.Status = newStatus;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }

      
        public async Task<ReimbursementRequestResponseDto?> GetByIdAsync(Guid id)
        {
            var r = await _repository.GetByIdAsync(id);

            if (r == null)
                return null;

            return new ReimbursementRequestResponseDto
            {
                ReimbursementRequestId = r.ReimbursementRequestId,
                Amount = r.Amount,
                Description = r.Description,
                Status = r.Status,
                AttachmentPath = r.AttachmentPath
            };
        }

        public async Task<IEnumerable<ReimbursementRequestResponseDto>> GetAllRequestsAsync()
        {
            var requests = await _repository.GetAllAsync();
            return requests.Select(r => new ReimbursementRequestResponseDto
            {
                ReimbursementRequestId = r.ReimbursementRequestId,
                Amount = r.Amount,
                Description = r.Description,
                Status = r.Status,
                AttachmentPath = r.AttachmentPath,
                CreatedAt = r.CreatedAt,
                EmployeeName = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : "Unknown",
                EmployeeEmail = r.User?.Email ?? string.Empty,
                EmployeeRole = r.User?.Role?.RoleName ?? string.Empty
            });
        }

        // DELETE
        public async Task<bool> DeleteReimbursementRequest(Guid id)
        {
            var request = await _repository.GetByIdAsync(id);

            if (request == null)
                return false;

            if (request.Status != ReimbursementStatusType.Submitted)
                throw new Exception("Only submitted requests can be deleted.");

            _repository.Delete(request);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
