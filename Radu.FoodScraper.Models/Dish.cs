using System;

namespace Radu.FoodScraper.Models
{
    public class Dish : Model<Guid>
    {
        public string MenuTitle { get; set; }
        public string MenuDescription { get; set; }
        public string MenuSectionTitle { get; set; }
        public string DishName { get; set; }
        public string DishDescription { get; set; }

        public virtual Scraper Scraper { get; set; }
    }
}
