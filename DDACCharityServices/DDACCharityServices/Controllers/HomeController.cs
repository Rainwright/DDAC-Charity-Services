using DDACCharityServices.Areas.Identity.Data;
using DDACCharityServices.Controllers.Enums;
using DDACCharityServices.Data;
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
        private readonly DDACCharityServicesForBackgroundContext _bgContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            UserManager<DDACCharityServicesUser> userManager,
            SignInManager<DDACCharityServicesUser> signInManager,
            DDACCharityServicesForBackgroundContext bgContext,
            ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _bgContext = bgContext;
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

                var users = _userManager.Users;
                HomeViewModel homeViewModel = new HomeViewModel();
                homeViewModel.currentUser = currentUser;

                if (User.IsInRole("Admin"))
                {
                    // there may be better way to do this but eh
                    var admin = await _userManager.GetUsersInRoleAsync(CustomUpdateUserModel.adminRoleName);
                    var staff = await _userManager.GetUsersInRoleAsync(CustomUpdateUserModel.staffRoleName);
                    var customer = await _userManager.GetUsersInRoleAsync(CustomUpdateUserModel.customerRoleName);

                    ViewBag.adminCount = admin.Count();
                    ViewBag.staffCount = staff.Count();
                    ViewBag.customerCount = customer.Count();

                    ViewBag.bgCount = _bgContext.Background.Count();
                    ViewBag.bgApprovedCount = _bgContext.Background.Where(backgrond => backgrond.BackgroundStatus == Statuses.Approved.ToString()).Count();
                    ViewBag.bgRejectedCount = _bgContext.Background.Where(backgrond => backgrond.BackgroundStatus == Statuses.Rejected.ToString()).Count();
                    ViewBag.bgPendingCount = _bgContext.Background.Where(backgrond => backgrond.BackgroundStatus == Statuses.Created.ToString()).Count();

                    List<Background> recentBackgrounds = _bgContext.Background.OrderByDescending(background => background.BackgroundID).Take(5).ToList();
                    List<Donation> recentDonations = _bgContext.Donation.OrderByDescending(donation => donation.DonationID).Take(20).ToList();

                    homeViewModel.recentBackgrounds = recentBackgrounds;
                    homeViewModel.recentDonations = recentDonations;
                }

                if(User.IsInRole("Staff"))
                {
                    List<Background> recentBackgrounds = _bgContext.Background
                        .Where(
                            background => background.CustomUserModelEmail.Equals(user.Email)
                        )
                        .OrderByDescending(
                            background => background.BackgroundID
                        )
                        .Take(5)
                        .ToList();

                    List<Donation> recentDonations = _bgContext.Donation
                        .Where(
                            donation => donation.Background.CustomUserModelEmail.Equals(user.Email)
                        )
                        .OrderByDescending(
                            donation => donation.DonationID
                        )
                        .Take(20)
                        .ToList();

                    homeViewModel.recentBackgrounds = recentBackgrounds;
                    homeViewModel.recentDonations = recentDonations;
                }

                if(User.IsInRole("Customer"))
                {
                    List<Background> recentBackgrounds = _bgContext.Background
                        .OrderByDescending(
                            background => background.BackgroundID
                        )
                        .Take(5)
                        .ToList();

                    List<Donation> recentDonations = _bgContext.Donation
                        .Where(
                            donation => donation.CustomerEmail.Equals(user.Email)
                        )
                        .OrderByDescending(
                            donation => donation.DonationID
                        )
                        .Take(20)
                        .ToList();

                    homeViewModel.recentBackgrounds = recentBackgrounds;
                    homeViewModel.recentDonations = recentDonations;
                }

                return View(homeViewModel);
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

    public class HomeViewModel
    {
        public HomeViewModel homeViewModel;
        public UserListModel currentUser;
        public List<Background> recentBackgrounds;
        public List<Donation> recentDonations;
    }
}
