using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    public class SearchController : Controller
    {
        private readonly IVideoService _videoService;
        private readonly IChannelService _channelService;
        private readonly UserManager<AppUser> _userManager;

        public SearchController(IVideoService videoService, IChannelService channelService, UserManager<AppUser> userManager)
        {
            _videoService = videoService;
            _channelService = channelService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return RedirectToAction("Index", "Home");

            var user = await _userManager.GetUserAsync(User);
            Guid? currentUserId = user?.Id;

            var videos = await _videoService.SearchVideosAsync(query, currentUserId);
            var channels = await _channelService.SearchChannelsAsync(query);

            ViewBag.Query = query;

            var results = (Channels: channels, Videos: videos);
            return View(results);
        }
    }
}