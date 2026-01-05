using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionDto>> GetAllSubscriptionsAsync();
        Task<SubscriptionDto> GetSubscriptionAsync(Guid subscriberUserId, Guid channelId);
        Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionDto dto);
        Task DeleteSubscriptionAsync(Guid subscriberUserId, Guid channelId);
        Task<bool> ToggleSubscriptionAsync(Guid subscriberUserId, Guid channelId);
        Task<int> GetSubscriberCountAsync(Guid channelOwnerUserId);
        Task<List<SubscriptionDto>> GetSubscriptionsBySubscriberAsync(Guid subscriberUserId);
        Task<SubscriptionDto?> GetSubscriptionStatusAsync(Guid subscriberUserId, Guid channelId);
        Task<bool> IsSubscribedAsync(Guid subscriberUserId, Guid channelOwnerUserId);
    }
}