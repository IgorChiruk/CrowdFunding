using System;
using System.Collections.Generic;

namespace kraud.Models
{
    public enum CompanyStatus : byte
    {
        InWork = 1,
        Finished = 2
    }
    public class Company
    {
        public Company() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Autor { get; set; }
        public int NeededAmount { get; set; }
        public int CollectedAmount { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime DateOfEnd { get; set; }
        public string ShortDescription { get; set; }
        public string History { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public CompanyStatus Status { get; set; }
        public ICollection<News> News { get; set; }
        public ICollection<Bonus> Bonuses { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}    