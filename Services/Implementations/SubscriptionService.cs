using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System; // Required for ArgumentException
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Repositories.Interfaces;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;
        private readonly IChannelService _channelService;
        private readonly IChannelRepository _channelRepository;
        private readonly ApplicationDbContext  _context;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IMapper mapper, IChannelService channelService, IChannelRepository channelRepository, ApplicationDbContext context)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
            _channelService = channelService;
            _channelRepository = channelRepository;
            _context = context;
        }

        public async Task<List<SubscriptionDto>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _subscriptionRepository.GetAllAsync();
            return _mapper.Map<List<SubscriptionDto>>(subscriptions);
        }

        public async Task<SubscriptionDto> GetSubscriptionAsync(Guid subscriberUserId, Guid channelId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriberUserId, channelId);
            if (subscription == null) throw new Exception("Subscription not found");
            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task<SubscriptionDto> CreateSubscriptionAsync(SubscriptionDto dto)
        {
            bool channelExists = await _channelService.ChannelExistsByOwnerIdAsync(dto.ChannelOwnerUserId);

            if (!channelExists)
            {
                throw new ArgumentException($"Channel owner with User ID {dto.ChannelOwnerUserId} does not exist in the Channels table. Subscription failed.");
            }

            var subscription = _mapper.Map<Subscription>(dto);
            subscription.SubscribedAt = DateTime.UtcNow;
            await _subscriptionRepository.AddAsync(subscription);
            return _mapper.Map<SubscriptionDto>(subscription);
        }

        public async Task DeleteSubscriptionAsync(Guid subscriberUserId, Guid channelId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriberUserId, channelId);
            if (subscription == null) throw new Exception("Subscription not found");
            await _subscriptionRepository.DeleteAsync(subscription);
        }

        public async Task<bool> ToggleSubscriptionAsync(Guid subscriberUserId, Guid channelId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByUsersAsync(subscriberUserId, channelId);

            if (subscription == null)
            {
                var newSubscription = new SubscriptionDto
                {
                    SubscriberUserId = subscriberUserId,
                    ChannelOwnerUserId = channelId,
                    SubscribedAt = DateTime.UtcNow
                };

                await CreateSubscriptionAsync(newSubscription);
                return true;
            }
            else
            {
                await DeleteSubscriptionAsync(subscriberUserId, channelId);
                return false;
            }
        }

        public async Task<int> GetSubscriberCountAsync(Guid channelOwnerUserId)
        {
            return await _subscriptionRepository.CountByChannelOwnerIdAsync(channelOwnerUserId);
        }
        public async Task<List<SubscriptionDto>> GetSubscriptionsBySubscriberAsync(Guid subscriberUserId)
        {
            var subscriptions = await _context.Subscriptions
                .Where(s => s.SubscriberUserId == subscriberUserId)
                .ToListAsync();

            var dtos = _mapper.Map<List<SubscriptionDto>>(subscriptions);

            foreach (var dto in dtos)
            {
                var channel = await _context.Channels
                    .Include(c => c.Owner)
                    .FirstOrDefaultAsync(c => c.OwnerUserId == dto.ChannelOwnerUserId);

                if (channel != null)
                {
                    dto.ChannelName = channel.Name;
                    dto.ProfilePicture = channel.Owner?.ProfileImagePath ?? "/images/default.png";
                }
            }

            return dtos;
        }
        public async Task<SubscriptionDto?> GetSubscriptionStatusAsync(Guid subscriberUserId, Guid channelId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByUsersAsync(subscriberUserId, channelId);

            return _mapper.Map<SubscriptionDto>(subscription);
        }
        public async Task<bool> IsSubscribedAsync(Guid subscriberUserId, Guid channelOwnerUserId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByUsersAsync(subscriberUserId, channelOwnerUserId);
            return subscription != null;
        }
    }
}