using Radu.FoodScraper.Scrapers.Dto;
using Radu.FoodScraper.Scrapers.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Radu.FoodScraper.Scraper
{
    public class PureScraperService : IScraperService
    {
        public async Task<IEnumerable<DishDto>> ScrapeAsync(string entryPointUrl)
        {
            var dummy = new List<DishDto>() { new DishDto { DishName = "dish1", DishDescription = "dish description 1", MenuDescription = "menu description 1", MenuSectionTitle = "menu section title 1", MenuTitle = "menu title" } };
            return await Task.FromResult(dummy);
        }
    }
}
