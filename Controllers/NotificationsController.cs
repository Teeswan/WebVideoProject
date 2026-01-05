using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService; 
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public NotificationsController(INotificationService notificationService, UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);

            await _notificationService.MarkAllAsReadAsync(user.Id);

            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(user.Id);

            return Ok(); 
        }

        [HttpPost]
        [Route("Notifications/Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _notificationService.DeleteNotificationAsync(id, user.Id);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Click(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == user.Id);

            if (notification == null) return NotFound();

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return Redirect(notification.LinkUrl);
        }
    }
}
