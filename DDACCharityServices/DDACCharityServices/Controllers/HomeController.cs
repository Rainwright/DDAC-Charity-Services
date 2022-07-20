using DDACCharityServices.Areas.Identity.Data;
using DDACCharityServices.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DDACCharityServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<DDACCharityServicesUser> _userManager;
        private readonly SignInManager<DDACCharityServicesUser> _signInManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            UserManager<DDACCharityServicesUser> userManager, 
            SignInManager<DDACCharityServicesUser> signInManager, 
            ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                var currentUser = new UserListModel();
                currentUser.CopyFromIdentityUser(user);
                await currentUser.SetRoleFromIdentityUser(user, _userManager);
                currentUser.FullImageUrl = user.profileImageUrl != null ? AWSHelper.GetFullImageUrl(user.profileImageUrl) : null;

                return View(currentUser);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
