using DDACCharityServices.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DDACCharityServices.Models
{
    public abstract class AbstractCustomUserClass
    {
        [Required(ErrorMessage = "Role is required.")]
        [Display(Name = "User Role")]
        public string UserRole { get; set; }

        public async Task SetRoleFromIdentityUser(DDACCharityServicesUser user, UserManager<DDACCharityServicesUser> _userManager)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0)
            {
                this.UserRole = roles[0];
            }
        }

        public abstract void CopyFromIdentityUser(DDACCharityServicesUser user);
    }
}
