using Microsoft.AspNet.Identity.EntityFramework;

namespace kraud.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Year { get; set; }
        public ApplicationUser()
        {
        }
    }
}