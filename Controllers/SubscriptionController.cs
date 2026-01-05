// File: Controllers/SubscriptionController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Services.Implementations;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IChannelService _channelService;

        public SubscriptionController(ISubscriptionService subscriptionService, UserManager<AppUser> userManager, IChannelService channelService)
        {
            _subscriptionService = subscriptionService;
            _userManager = userManager;
            _channelService = channelService;
        }

        [HttpPost("Toggle")]
        public async Task<IActionResult> ToggleSubscription([FromForm] string channelId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (!Guid.TryParse(channelId, out Guid channelGuid))
            {
                return BadRequest("Invalid Channel ID format.");
            }

            Guid subscriberGuid = user.Id;

            try
            {
                bool isSubscribed = await _subscriptionService.ToggleSubscriptionAsync(
                    subscriberGuid,
                    channelGuid
                );

                int subscriberCount = await _subscriptionService.GetSubscriberCountAsync(channelGuid);

                return Ok(new
                {
                    isSubscribed = isSubscribed,
                    subscriberCount = subscriberCount
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    code = "ChannelNotFound"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An unexpected server error occurred during subscription toggle.",
                    details = ex.Message
                });
            }
        }

        [Authorize]
        public async Task<IActionResult> GetMySubscriptions()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var subscriptions = await _subscriptionService.GetSubscriptionsBySubscriberAsync(currentUser.Id);

            foreach (var sub in subscriptions)
            {
                var channel = await _channelService.GetChannelByUserIdAsync(sub.ChannelOwnerUserId);

                sub.ChannelName = channel?.Name ?? "Channel Not Found";

                sub.ChannelId = channel?.ChannelId ?? Guid.Empty;
            }

            return View(subscriptions);
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetSubscriberCount([FromQuery] string channelId)
        {
            if (!Guid.TryParse(channelId, out Guid channelOwnerGuid))
            {
                return BadRequest("Invalid Channel ID format.");
            }

            try
            {
                int subscriberCount = await _subscriptionService.GetSubscriberCountAsync(channelOwnerGuid);

                return Ok(new
                {
                    subscriberCount = subscriberCount
                });
            }
            catch (Exception)
            {
                return Ok(new { subscriberCount = 0 });
            }
        }
    }
}