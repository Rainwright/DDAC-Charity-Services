using DDACCharityServices.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace DDACCharityServices.Models
{
    public class UserListModel : AbstractCustomUserClass
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public override void CopyFromIdentityUser(DDACCharityServicesUser user)
        {
            this.Id = user.Id;
            this.Email = user.Email;
            this.UserName = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
        }
    }
}
