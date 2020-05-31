using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace kraud.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        public bool IsAdmin { get; set; }

        public ICollection<Company> Companies { get; set; }
    }
}

