using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MagicInventoryWebsite.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        // Can add additional data and relationships to other classes here, e.g., 0 to 1 relationship.
        // As an example of a 0 to 1 relationship:
        //public int? StoreID { get; set; }
        //public Store Store { get; set; }


    }
}
