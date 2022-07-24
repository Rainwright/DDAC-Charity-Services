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
using Amazon;
using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

using Microsoft.Extensions.Configuration;
using System.IO;

namespace DDACCharityServices.Areas.Identity.Pages.Account.Manage
{
    public class EmailSubscriptionModel : PageModel
    {
        private readonly UserManager<DDACCharityServicesUser> _userManager;
        private readonly SignInManager<DDACCharityServicesUser> _signInManager;

        public bool subscribed { get; set; } = false;

        [TempData]
        public string ActionMessage { get; set; }

        public EmailSubscriptionModel(UserManager<DDACCharityServicesUser> userManager, SignInManager<DDACCharityServicesUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public bool testFunc() {
            return true;
		}

        public async Task<IActionResult> OnGetAsync() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

			if (user.SubscriptionArn == null) {
				subscribed = false;
                HttpContext.Session.SetString("subscribed", "false");
				return Page();
			}

			var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();


            var res = await AWSHelper.GetAWSSimpleNotificationServiceClient().GetSubscriptionAttributesAsync(user.SubscriptionArn);
			if (res.Attributes["PendingConfirmation"].Equals("true")) {
                subscribed = false;
                HttpContext.Session.SetString("subscribed", "false");
            } else {
                subscribed = true;
                HttpContext.Session.SetString("subscribed", "true");
            }

            return Page();
        }

        public async Task OnPostAsync() {
            var user = await _userManager.GetUserAsync(User);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            if (HttpContext.Session.GetString("subscribed").Equals("false")) {
                SubscribeRequest subscribeRequest = new SubscribeRequest() {
                    Endpoint = User.Identity.Name,
                    Protocol = "email",
                    TopicArn = configure["AWSCredential:TopicArn"],
                    ReturnSubscriptionArn = true,
                    Attributes = {
                    { "FilterPolicy", "{\"Email\": [\"zhen.yang.ching@gmail.com\"]}" }
                }
                };
                SubscribeResponse result = await AWSHelper.GetAWSSimpleNotificationServiceClient().SubscribeAsync(subscribeRequest);
                
                user.SubscriptionArn = result.SubscriptionArn;
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                //ActionMessage = "An activation email has been sent to your mailbox. Please confirm your subscription by clicking the link in the email.";
                TempData["ActionMessage"] = "An activation email has been sent to your mailbox. Please confirm your subscription by clicking the link in the email.";
            } else {
                UnsubscribeRequest unsubscribeRequest = new UnsubscribeRequest(user.SubscriptionArn);
                await AWSHelper.GetAWSSimpleNotificationServiceClient().UnsubscribeAsync(unsubscribeRequest);

                user.SubscriptionArn = null;
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                //ActionMessage = "You have been unsubscribed from email notification.";
                TempData["ActionMessage"] = "You have been unsubscribed from email notification.";
                HttpContext.Session.SetString("subscribed", "false");
            }
        }
    }
}
