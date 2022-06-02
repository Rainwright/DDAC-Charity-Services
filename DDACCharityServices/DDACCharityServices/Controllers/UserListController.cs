﻿using DDACCharityServices.Areas.Identity.Data;
using DDACCharityServices.Data;
using DDACCharityServices.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DDACCharityServices.Controllers
{
    public class UserListController : Controller
    {
        private readonly UserManager<DDACCharityServicesUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<DDACCharityServicesUser> _signInManager;
        private readonly DDACCharityServicesContext _context;

        public UserListController(
            DDACCharityServicesContext context,
            UserManager<DDACCharityServicesUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;

            var userList = new List<UserListModel>();
            foreach (DDACCharityServicesUser user in users)
            {
                var currentUser = new UserListModel();
                currentUser.CopyFromIdentityUser(user);
                await currentUser.SetRoleFromIdentityUser(user, _userManager);
                userList.Add(currentUser);
            }
            return View(userList);
        }

        public IActionResult Create()
        {
            CustomUserModel user = new CustomUserModel();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomUserModel model)
        {
            if (ModelState.IsValid)
            {
                var checkEmailExists = await _userManager.FindByEmailAsync(model.Email);

                if(checkEmailExists == null)
                {
                    bool roleExists = await _roleManager.RoleExistsAsync(model.UserRole);
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.UserRole));
                    }

                    var user = new DDACCharityServicesUser
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        EmailConfirmed = true
                    };

                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded && setPhoneResult != null)
                    {
                        await _userManager.AddToRoleAsync(user, model.UserRole);

                        return RedirectToAction(nameof(Index));
                    } else
                    {
                        return View(model);
                    }
                } else
                {
                    TempData["message"] = $"Existing User!";
                    return View(model);
                }
            } else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var newUser = new CustomUpdateUserModel();
                newUser.CopyFromIdentityUser(user);
                await newUser.SetRoleFromIdentityUser(user, _userManager);
                return View(newUser);
            } else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomUpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{model.Id}'.");
            }

            if (!ModelState.IsValid)
            {
                TempData["message"] = "The form is not valid! Please try again!";
                return View(model);
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    TempData["message"] = "Unexpected error when trying to set phone number.";
                    return View(model);
                }
            }

            if (model.FirstName != user.FirstName)
            {
                user.FirstName = model.FirstName;
            }

            if (model.LastName != user.LastName)
            {
                user.LastName = model.LastName;
            }

            var currentUserRole = await _userManager.GetRolesAsync(user);

            bool roleExists = await _roleManager.RoleExistsAsync(model.UserRole);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(model.UserRole));
            }

            if (currentUserRole.IndexOf(model.UserRole) == -1)
            {
                foreach(var role in currentUserRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }

                await _userManager.AddToRoleAsync(user, model.UserRole);
            }

            await _userManager.UpdateAsync(user);
            TempData["message"] = "Profile has been updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var newUser = new CustomUserModel();
                newUser.CopyFromIdentityUser(user);
                await newUser.SetRoleFromIdentityUser(user, _userManager);
                return View(newUser);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
