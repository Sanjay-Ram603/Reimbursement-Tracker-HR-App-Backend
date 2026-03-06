namespace ReimbursementTrackerApp.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(Guid userId, string message);
    }
}
