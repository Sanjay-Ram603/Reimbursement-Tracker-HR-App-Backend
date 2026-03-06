using ReimbursementTrackerApp.Models.Notification;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task SendNotificationAsync(Guid userId, string message)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();
        }


    }
}
