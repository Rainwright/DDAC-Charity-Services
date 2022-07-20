using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DDACCharityServices.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace DDACCharityServices.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<DDACCharityServicesUser> _userManager;
        private readonly SignInManager<DDACCharityServicesUser> _signInManager;
        //private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(
            UserManager<DDACCharityServicesUser> userManager,
            SignInManager<DDACCharityServicesUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "The form is not valid! Please try again!";
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                StatusMessage = "Invalid email address.";
                return Page();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var changePasswordResult = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            return RedirectToPage("Login");

            //if (ModelState.IsValid)
            //{

            //    //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            //    //{
            //    //    // Don't reveal that the user does not exist or is not confirmed
            //    //    return RedirectToPage("./ForgotPasswordConfirmation");
            //    //}

            //    //// For more information on how to enable account confirmation and password reset please 
            //    //// visit https://go.microsoft.com/fwlink/?LinkID=532713
            //    //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            //    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //    //var callbackUrl = Url.Page(
            //    //    "/Account/ResetPassword",
            //    //    pageHandler: null,
            //    //    values: new { area = "Identity", code },
            //    //    protocol: Request.Scheme);

            //    //await _emailSender.SendEmailAsync(
            //    //    Input.Email,
            //    //    "Reset Password",
            //    //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            //    //return RedirectToPage("./ForgotPasswordConfirmation");
            //}
        }
    }
}
