using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JustLearn1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace JustLearn1
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> UsersWithRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoleViewModel = new List<Users_in_Role>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoleViewModel.Add(new Users_in_Role
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = string.Join(",", roles)
                });
            }
            return View(userRoleViewModel);
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();


            var model = new Users_in_Role
            {
                Email = user.Email,
                UserId = user.Id,
                Username = user.UserName,

                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };


            ViewData["Roles"] = new SelectList(roles, "Name", "Name", model.Role); // model.Role varsayılan seçili değeri sağlar.

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Users_in_Role model)
        {

            var rolesList = await _roleManager.Roles.ToListAsync();
            ViewData["Roles"] = new SelectList(rolesList, "Name", "Name");

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Username;


            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
            var result = await _userManager.AddToRoleAsync(user, model.Role);


            if (!result.Succeeded)
            {

                ModelState.AddModelError(string.Empty, "Error updating role");
                return View(model);
            }

            await _userManager.UpdateAsync(user);

            return RedirectToAction("UsersWithRoles");
        }
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {

                ModelState.AddModelError("", "Kullanıcı bulunamadı");
                return View("Error");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("Error");
            }

            TempData["Message"] = "Kullanıcı başarı ile silindi";
            return RedirectToAction("UsersWithRoles");

        }


        public async Task<IActionResult> CreateAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");

            return View();
        }

       



        [HttpPost]
        
        public async Task<IActionResult> Create(IdentityUser user, string password, string role)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");

            if (ModelState.IsValid)
            {

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {

                    var addedToRole = await _userManager.AddToRoleAsync(user, role);
                    return RedirectToAction("Index");
                }
                else
                {

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View();
        }



    }
}
