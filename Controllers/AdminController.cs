using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IChannelService _channelService;

        public AdminController(
            IVideoService videoService,
            UserManager<AppUser> userManager,
            IChannelService channelService)
        {
            _videoService = videoService;
            _userManager = userManager;
            _channelService = channelService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var channel = await _channelService.GetChannelByUserIdAsync(currentUser.Id);
            if (channel == null)
            {
                return RedirectToAction("Create", "Channel");
            }

            var allVideos = await _videoService.GetAllVideosAsync();
            var creatorVideos = allVideos
                .Where(v => v.ChannelId == channel.ChannelId)
                .ToList();

            foreach (var v in creatorVideos)
            {
                v.LikeCount = await _videoService.GetLikeCountAsync(v.VideoId);
            }

            long totalViews = creatorVideos.Sum(v => v.ViewCount);
            long totalLikes = creatorVideos.Sum(v => v.LikeCount); 

            var model = new AdminDashboardDto
            {
                TotalVideos = creatorVideos.Count,
                TotalUsers = _userManager.Users.Count(),
                TotalChannels = 1,
                TotalViews = totalViews,
                TotalLikes = totalLikes,
                RecentVideos = creatorVideos.OrderByDescending(v => v.CreatedAt).Take(5).ToList()
            };

            return View(model);
        }
    }
}