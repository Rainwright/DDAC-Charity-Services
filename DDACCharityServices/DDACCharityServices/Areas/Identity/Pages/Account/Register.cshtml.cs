using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using DDACCharityServices.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDACCharityServices.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<DDACCharityServicesUser> _signInManager;
        private readonly UserManager<DDACCharityServicesUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        static string customerRoleName = "Customer";
        static string staffRoleName = "Staff";
        static string adminRoleName = "Admin";

        public SelectList RoleSelectList = new SelectList(
            new List<SelectListItem>
            {
                new SelectListItem { Selected=true, Text="Select Role", Value=""},
                new SelectListItem { Selected=false, Text=customerRoleName, Value=customerRoleName},
                new SelectListItem { Selected=false, Text=staffRoleName, Value=staffRoleName},
            },
            "Value", "Text", 1
           );

        public RegisterModel(
            UserManager<DDACCharityServicesUser> userManager,
            SignInManager<DDACCharityServicesUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "First Name is required.")]
            [StringLength(100, ErrorMessage = "{0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            [StringLength(100, ErrorMessage = "{0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Role is required.")]
            [Display(Name = "User Role")]
            public string UserRole { get; set; }
        }

        public async Task<IActionResult> OnPostRegisterTestAdmin()
        {
            // add register test admin user code here
            string adminEmail = "admin@mail.com";
            string adminPassword = "Thisisasecureadminpassword123@_";
            string adminName = "admin";

            bool adminRoleExists = await _roleManager.RoleExistsAsync(adminRoleName);
            if (!adminRoleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(adminRoleName));

            }

            var user = new DDACCharityServicesUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                FirstName = adminName,
                LastName = adminName,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, adminRoleName);

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    /*return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });*/
                    TempData["message"] = $"Successfully created test admin account with || Email: {adminEmail} || Password: {adminPassword}";
                    return RedirectToPage("Login");
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(Url.Content("~/"));
                }
            }

            // existing test admin account since the email is duplicated
            TempData["message"] = $"There is already an existing test admin account!";
            return Page();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                bool customerRoleExists = await _roleManager.RoleExistsAsync(customerRoleName);
                if(!customerRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(customerRoleName));

                }

                bool staffRoleExists = await _roleManager.RoleExistsAsync(staffRoleName);
                if(!staffRoleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(staffRoleName));

                }

                var user = new DDACCharityServicesUser { 
                    Email = Input.Email, 
                    UserName = Input.Email,
                    FirstName = Input.FirstName, 
                    LastName = Input.LastName,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Input.UserRole);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        /*return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });*/
                        return RedirectToPage("Login");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
