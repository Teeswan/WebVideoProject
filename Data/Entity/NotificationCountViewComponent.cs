using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.Data.Entity; 
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.ViewComponents 
{
    public class NotificationCountViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<AppUser> _userManager;

        public NotificationCountViewComponent(INotificationService notificationService, UserManager<AppUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
            if (user == null) return View(0);

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            int unreadCount = notifications.Count(n => !n.IsRead);

            return View(unreadCount);
        }
    }
}