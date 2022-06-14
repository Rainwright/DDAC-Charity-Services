using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using DDACCharityServices.Areas.Identity.Data;
using DDACCharityServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DDACCharityServices.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<DDACCharityServicesUser> _userManager;
        private readonly SignInManager<DDACCharityServicesUser> _signInManager;
        private const string userS3Directory = "/user";

        public IndexModel(
            UserManager<DDACCharityServicesUser> userManager,
            SignInManager<DDACCharityServicesUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public string ProfileImageURL{ get; set; }

        [TempData]
        public string ProfileStatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "First Name is required.")]
            [StringLength(100, ErrorMessage = "{0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            [StringLength(100, ErrorMessage = "{0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            public string LastName { get; set; }
        }

        private async Task LoadAsync(DDACCharityServicesUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string imageKey = null;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user.profileImageUrl != null)
            {
                imageKey = user.profileImageUrl;
            }

            await LoadAsync(user);

            if(imageKey != null)
            {
                ProfileImageURL = AWSHelper.GetFullImageUrl(imageKey);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                /*await LoadAsync(user);
                return Page();*/
                return BadRequest("The form is not valid! Please try again!");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
            }

            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUploadImage(IFormFile imageFile)
        {
            Debug.WriteLine(imageFile);

            AmazonS3Client S3Client = AWSHelper.GetAWSCredentialS3Object();

            string ValidationErrors = AWSHelper.ValidateImage(imageFile);

            if (ValidationErrors == null)
            {
                try
                {
                    string filename = await AWSHelper.UploadImage(S3Client, imageFile, userS3Directory);
                    string imageKey = AWSHelper.GetImageKey(filename, userS3Directory);

                    //string imageurl = "https://" + image.BucketName + ".s3.amazonaws.com/" + image.Key

                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                    }

                    if(user.profileImageUrl == null)
                    {
                        user.profileImageUrl = imageKey;
                    } else
                    {
                        await AWSHelper.DeleteImage(S3Client, user.profileImageUrl, userS3Directory);
                        user.profileImageUrl = imageKey;
                    }

                    await _userManager.UpdateAsync(user);

                    await _signInManager.RefreshSignInAsync(user);

                    ProfileImageURL = AWSHelper.GetFullImageUrl(imageKey);
                    ProfileStatusMessage = "Successfully set your profile image!";

                    return RedirectToPage();
                }
                catch (Exception ex)
                {
                    return BadRequest("The image file is invalid");
                }
            } else
            {
                return BadRequest(ValidationErrors);
            }
        }

        public async Task<IActionResult> OnPostDeleteImage()
        {
            AmazonS3Client S3Client = AWSHelper.GetAWSCredentialS3Object();

            string imageKey = null;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user.profileImageUrl != null)
            {
                await AWSHelper.DeleteImage(S3Client, user.profileImageUrl, userS3Directory);
                user.profileImageUrl = imageKey;
                imageKey = user.profileImageUrl;
                user.profileImageUrl = null;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            ProfileStatusMessage = "Successfully removed your profile image!";
            return RedirectToPage();
        }
    }
}
