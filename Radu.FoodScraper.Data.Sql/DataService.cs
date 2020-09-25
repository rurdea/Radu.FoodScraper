using Microsoft.EntityFrameworkCore;
using Radu.FoodScraper.Data.Interfaces;
using Radu.FoodScraper.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Radu.FoodScraper.Data.Sql
{
    public class DataService : IDataService
    {
        protected Context.DbContext Context { get; private set; }
        public DataService(string connectionString)
        {
            Context = new Context.DbContext(connectionString);
        }

        public async Task PersistAsync(string scraperType, System.Collections.Generic.IEnumerable<Dish> dishes)
        {
            var scraper = await Context.Scrapers.Where(s => s.Name.ToLower() == scraperType.ToLower()).FirstOrDefaultAsync();
            if (scraper == null)
            {
                scraper = new Scraper() { Id = Guid.NewGuid(), Name = scraperType, CreatedDateTime = DateTime.UtcNow };
                Context.Scrapers.Add(scraper);
            }

            foreach(var dish in dishes)
            {
                var dbDish = await Context.Dishes.Where(d => d.ScraperId == scraper.Id && d.DishName.ToLower() == dish.DishName.ToLower()).FirstOrDefaultAsync();
                if (dbDish == null)
                {
                    dbDish = new Dish() { Id = Guid.NewGuid(), CreatedDateTime = DateTime.UtcNow, ScraperId = scraper.Id };
                    Context.Dishes.Add(dbDish);
                }
                else
                {
                    dbDish.LastUpdatedDateTime = DateTime.UtcNow;
                }
                dbDish.DishDescription = dish.DishDescription;
                dbDish.DishName = dish.DishName;
                dbDish.MenuDescription = dish.MenuDescription;
                dbDish.MenuSectionTitle = dish.MenuSectionTitle;
                dbDish.MenuTitle = dish.MenuTitle;
            }

            await Context.SaveChangesAsync();
        }
    }
}
