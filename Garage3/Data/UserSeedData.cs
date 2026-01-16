using Garage3.Models;
using Microsoft.AspNetCore.Identity;

namespace Garage3.Data
{
    internal class UserSeedData
    {
        private static ApplicationDbContext _context = default!;
        private static RoleManager<IdentityRole> _roleManager = default!;
        private static UserManager<ApplicationUser> _userManager = default!;

        public static async Task<IEnumerable<ApplicationUser>> Init(ApplicationDbContext context, IServiceProvider services)
        {
            _context = context;

            _roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleNames = new[] { "Member", "Admin" };
            foreach (var role in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@admin.com";
            var memberEmail = "member@member.com";

            var admin = await AddAccountAsync(adminEmail, "Admin", "Johansoon", "199010112382", "Test.qrw1!");
            var member = await AddAccountAsync(memberEmail, "Member", "Smith", "199912122380", "Test.qrw2!");

            await AddUserToRoleAsync(admin, "Admin");
            await AddUserToRoleAsync(member, "Member");

            return new[] { admin, member };

        }

        private static async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
        {
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
            }
        }

        private static async Task<ApplicationUser> AddAccountAsync(string accountEmail, string fName, string lName, string ssn, string pw)
        {
            var found = await _userManager.FindByNameAsync(accountEmail);

            if (found != null) return found;

            var user = new ApplicationUser
            {
                UserName = accountEmail,
                Email = accountEmail,
                FirstName = fName,
                LastName = lName,
                SSN = ssn,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, pw);

            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));

            return user;
        }

        private static async Task AddRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (await _roleManager.RoleExistsAsync(roleName)) continue;

                var role = new IdentityRole { Name = roleName };

                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                    throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
