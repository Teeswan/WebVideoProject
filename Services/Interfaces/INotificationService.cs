using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
        Task CreateNotificationAsync(Guid userId, string message, string? linkUrl, string? actorImg, string? videoThumb);
        Task DeleteNotificationAsync(Guid notificationId, Guid userId);
    }
}
