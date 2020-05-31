using System.Data.Entity;

namespace kraud.Models
{
    public class CompanyContext : DbContext
    {

        public CompanyContext() : base("IdentityDb")
        {
        }
        public DbSet<News> News { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<Company> Companies { get; set; }
    }
}