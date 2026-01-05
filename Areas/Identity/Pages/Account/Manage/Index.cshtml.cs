using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            public string? PhoneNumber { get; set; }

            public IFormFile? ProfileImage { get; set; }

            public string? ExistingProfileImage { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        private async Task LoadAsync(AppUser user)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                ExistingProfileImage = user.ProfileImagePath
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("User not found");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("User not found");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // ============================
            // PROFILE IMAGE UPLOAD LOGIC
            // ============================
            if (Input.ProfileImage != null)
            {
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/users");
                Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid() + Path.GetExtension(Input.ProfileImage.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await Input.ProfileImage.CopyToAsync(stream);

                // delete old image
                if (!string.IsNullOrEmpty(user.ProfileImagePath))
                {
                    var oldFile = Path.Combine("wwwroot", user.ProfileImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }

                user.ProfileImagePath = "/uploads/users/" + fileName;
            }

            await _userManager.UpdateAsync(user);

            StatusMessage = "Your profile has been updated!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePhotoAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(user.ProfileImagePath))
            {
                var fullPath = Path.Combine("wwwroot", user.ProfileImagePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                user.ProfileImagePath = null;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToPage();
        }
    }
}
