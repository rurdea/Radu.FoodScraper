using System;

namespace Radu.FoodScraper.Models
{
    // to do: create separate model for menu
    public class Dish : Model<Guid>
    {
        public string MenuTitle { get; set; }
        public string MenuDescription { get; set; }
        public string MenuSectionTitle { get; set; }
        public string DishName { get; set; }
        public string DishDescription { get; set; }

        public Guid ScraperId { get; set; }
        public virtual Scraper Scraper { get; set; }
    }
}
