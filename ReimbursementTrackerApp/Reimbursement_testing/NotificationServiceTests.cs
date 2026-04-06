using Moq;
using ReimbursementTrackerApp.Models.Notification;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;

namespace Reimbursement_testing
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _repoMock;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _repoMock = new Mock<INotificationRepository>();
            _service = new NotificationService(_repoMock.Object);
        }

        [Fact]
        public async Task SendNotificationAsync_CreatesAndSavesNotification()
        {
          
            var userId = Guid.NewGuid();
            var message = "Your request has been approved.";
            Notification? saved = null;

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
                .Callback<Notification>(n => saved = n)
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            await _service.SendNotificationAsync(userId, message);

            Assert.NotNull(saved);
            Assert.Equal(userId, saved!.UserId);
            Assert.Equal(message, saved.Message);
            Assert.False(saved.IsRead);
        }

        [Fact]
        public async Task SendNotificationAsync_NotificationIsUnread()
        {
         
            var userId = Guid.NewGuid();
            Notification? saved = null;

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
                .Callback<Notification>(n => saved = n)
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

           
            await _service.SendNotificationAsync(userId, "Test message");

           
            Assert.False(saved!.IsRead);
        }

        [Fact]
        public async Task SendNotificationAsync_NotificationIdIsNotEmpty()
        {
           
            Notification? saved = null;

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
                .Callback<Notification>(n => saved = n)
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.SendNotificationAsync(Guid.NewGuid(), "Hello");

            Assert.NotEqual(Guid.Empty, saved!.NotificationId);
        }

        [Fact]
        public async Task SendNotificationAsync_CallsSaveChanges()
        {
            
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            await _service.SendNotificationAsync(Guid.NewGuid(), "Test");

            
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SendNotificationAsync_DifferentMessages_EachSavedCorrectly()
        {
            
            var userId = Guid.NewGuid();
            var messages = new List<string>();

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
                .Callback<Notification>(n => messages.Add(n.Message))
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            await _service.SendNotificationAsync(userId, "Message 1");
            await _service.SendNotificationAsync(userId, "Message 2");

            
            Assert.Contains("Message 1", messages);
            Assert.Contains("Message 2", messages);
        }
    }
}
