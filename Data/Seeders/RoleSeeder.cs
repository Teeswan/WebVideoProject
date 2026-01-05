using Microsoft.AspNetCore.Identity;
using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.Data.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles = { "User", "Admin", "SuperAdmin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }

        public static async Task SeedSuperAdminAsync(UserManager<AppUser> userManager)
        {
            string email = "superadmin@example.com";
            string password = "12345";  

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = email,      
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = "Super Admin"
                };

                var create = await userManager.CreateAsync(admin, password);

                if (!create.Succeeded)
                {
                    await userManager.CreateAsync(admin);
                    await userManager.AddPasswordAsync(admin, password);
                }
            }
            else
            {
                if (!admin.EmailConfirmed)
                {
                    admin.EmailConfirmed = true;
                    await userManager.UpdateAsync(admin);
                }

                if (!await userManager.HasPasswordAsync(admin))
                {
                    await userManager.AddPasswordAsync(admin, password);
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "SuperAdmin"))
                await userManager.AddToRoleAsync(admin, "SuperAdmin");

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                await userManager.AddToRoleAsync(admin, "Admin");

            if (!await userManager.IsInRoleAsync(admin, "User"))
                await userManager.AddToRoleAsync(admin, "User");
        }
    }
}