using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Controllers
{
    [Authorize(Roles = "SuperAdmin")] 
    public class SuperAdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager; 

        public SuperAdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var totalUsers = _userManager.Users.Count();

            var model = new SuperAdminDashboardDto
            {
                TotalAdmins = admins.Count,
                TotalUsers = totalUsers,
                AdminUsers = admins.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                    await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BanUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                    await _userManager.RemoveFromRoleAsync(user, "SuperAdmin");

                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(1));
                TempData["SuccessMessage"] = $"User {user.UserName} has been permanently banned.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                TempData["SuccessMessage"] = $"Admin role revoked from {user.UserName}.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}