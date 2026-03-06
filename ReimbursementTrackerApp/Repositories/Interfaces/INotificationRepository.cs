
using ReimbursementTrackerApp.Models.Notification;

namespace ReimbursementTrackerApp.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
        Task SaveChangesAsync();
    }
}
