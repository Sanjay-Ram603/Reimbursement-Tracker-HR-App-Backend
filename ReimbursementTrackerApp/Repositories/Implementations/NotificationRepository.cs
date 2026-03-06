using Microsoft.EntityFrameworkCore;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.Models.Notification;
using ReimbursementTrackerApp.Repositories.Interfaces;

namespace ReimbursementTrackerApp.Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ReimbursementDbContext _context;

        public NotificationRepository(ReimbursementDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
