using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Models;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVideoService _videoService;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, IVideoService videoService, ApplicationDbContext context)
        {
            _logger = logger;
            _videoService = videoService;
            _context = context;
        }

        public async Task<IActionResult> Index(Guid? categoryId)
        {
            var allVideos = await _videoService.GetAllVideosAsync();

            var filteredVideos = categoryId.HasValue
                ? allVideos.Where(v => v.CategoryId == categoryId.Value)
                : allVideos;

            var displayVideos = filteredVideos
                .Where(v => v.Visibility.ToLower() == "public")
                .ToList();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategory = categoryId;

            return View(displayVideos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
