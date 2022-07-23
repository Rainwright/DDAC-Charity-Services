using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DDACCharityServices.Models
{
    public class CustomBackgroundModel
    {
        public int BackgroundID { get; set; }

        [Display(Name = "Title of Donation")]
        public string BackgroundName { get; set; }

        [Display(Name = "Description")]
        public string BackgroundDescription { get; set; }

        [Display(Name = "Amount Requested")]
        public int BackgroundAmount { get; set; }

        [Display(Name = "Account Requesting for Donation")]
        public string CustomUserModelEmail { get; set; }

        [Display(Name = "Status of Donation")]
        public string BackgroundStatus { get; set; }

        [Display(Name = "Total Donations Gathered")]
        public int TotalDonations { get; set; }

        public void CopyFromBackground(Background bg)
        {
            BackgroundID = bg.BackgroundID;
            BackgroundName = bg.BackgroundName;
            BackgroundDescription = bg.BackgroundDescription;
            BackgroundAmount = bg.BackgroundAmount;
            CustomUserModelEmail = bg.CustomUserModelEmail;
            BackgroundStatus = bg.BackgroundStatus;
        }
    }
}
