namespace kraud.Models
{
    public class Bonus
    {
        public Bonus() { }

        public int Id { get; set; }
        public int BonusPrise { get; set; }

        public string BonusText { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}