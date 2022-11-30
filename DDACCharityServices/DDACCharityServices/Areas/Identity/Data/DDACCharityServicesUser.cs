using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DDACCharityServices.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the DDACCharityServicesUser class
    public class DDACCharityServicesUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        
        [PersonalData]
        public string LastName { get; set; }
        
        public string profileImageUrl { get; set; }

        public string SubscriptionArn { get; set; }
    }
}
