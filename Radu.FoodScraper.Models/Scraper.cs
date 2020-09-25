using System;
using System.Collections.Generic;

namespace Radu.FoodScraper.Models
{
    public class Scraper : Model<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Urls { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; }
    }
}
