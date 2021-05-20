using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagerExample.Models;

namespace UserManagerExample.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    [Area("Admin")]
    public class UserRolesController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserRolesController(UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var currentVM = new UserRolesViewModel()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Fname = user.Fname,
                    Sname = user.Sname,
                    Roles = new List<string>(await _userManager.GetRolesAsync(user))
                };

                userRolesViewModel.Add(currentVM);
            }

            return View(userRolesViewModel);
        }

        //GET
        public async Task<IActionResult> Manage(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new List<ManageUserRolesViewModel>();

            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Selected = false
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }

                model.Add(userRolesViewModel);
                    
            }

            return View(model);
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove roles from user.");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(s => s.RoleName));


            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add roles from user.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
