using Garage3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Garage3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userNanager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userNamager, RoleManager<IdentityRole> roleManager)
        {
            _userNanager = userNamager;
            _roleManager = roleManager;
        }


        //List users and roles
        public async Task<IActionResult> Users()
        {
            var users = _userNanager.Users.ToList();
            var model = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userNanager.GetRolesAsync(user);

                model.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    CurrentRole = roles.FirstOrDefault() ?? "None"
                });
            }

            return View(model);

        }

        ////change role
        //[HttpPost]

        //public async Task<IActionResult> ChangeRole(string userId, string role)
        //{
        //    var user = await _userNanager.FindByIdAsync(userId);

        //}


    }
}
