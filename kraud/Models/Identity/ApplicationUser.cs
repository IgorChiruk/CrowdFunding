using kraud.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace kraud.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        public bool IsAdmin { get; set; }
    }
}

