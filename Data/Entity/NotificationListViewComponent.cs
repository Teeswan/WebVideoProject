using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Data.Entity; 
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.ViewComponents 
{
    [ViewComponent(Name = "NotificationList")]
    public class NotificationListViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<AppUser> _userManager;

        public NotificationListViewComponent(INotificationService notificationService, UserManager<AppUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);
            if (user == null) return View(new List<NotificationDto>());

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            return View(notifications.Take(5).ToList());
        }
    }
}   