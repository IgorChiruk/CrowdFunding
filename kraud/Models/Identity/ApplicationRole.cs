using Microsoft.AspNet.Identity.EntityFramework;

namespace kraud.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() { }

        public string Description { get; set; }
    }
}