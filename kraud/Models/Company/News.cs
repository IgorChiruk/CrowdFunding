namespace kraud.Models
{
    public class News
    {
        public News() { }

        public int Id { get; set; }
        public string NewsText { get; set; }
        public string NewsImage { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

    }
}