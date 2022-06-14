using DDACCharityServices.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DDACCharityServices.Models
{
    public class CustomUpdateUserModel : AbstractCustomUserClass
    {
        public string Id { get; set; }

        static string customerRoleName = "Customer";
        static string staffRoleName = "Staff";
        static string adminRoleName = "Admin";

        public SelectList FullRoleSelectList = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Selected=true, Text="Select Role", Value=""},
                new SelectListItem { Selected=false, Text=customerRoleName, Value=customerRoleName},
                new SelectListItem { Selected=false, Text=staffRoleName, Value=staffRoleName},
                new SelectListItem { Selected=false, Text=adminRoleName, Value=adminRoleName},
                },
                "Value", "Text", 1
               );

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public override void CopyFromIdentityUser(DDACCharityServicesUser user)
        {
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.PhoneNumber = user.PhoneNumber;
        }
    }
}
