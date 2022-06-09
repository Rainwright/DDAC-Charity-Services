using System.ComponentModel.DataAnnotations;

namespace DDACCharityServices.Models
{
    public class Background
    {
        public int BackgroundID { get; set; }
        [Required(ErrorMessage = "Donation Request must have title!")]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Title of Donation")]
        public string BackgroundName { get; set; }
        [Required(ErrorMessage = "Donation Request must have a short description!")]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Description")]
        public string BackgroundDescription { get; set; }
        [Display(Name = "Amount Requested")]
        [Required(ErrorMessage = "Please specify an amount required for this donation to happen!")]
        public int BackgroundAmount { get; set; }
        [Display(Name = "Account Requesting for Donation")]
        public string CustomUserModelEmail { get; set; }

        [Display(Name = "Status of Donation")]
        public string BackgroundStatus { get; set; }
    }
}
