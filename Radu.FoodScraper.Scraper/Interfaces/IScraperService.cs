using Radu.FoodScraper.Scrapers.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Radu.FoodScraper.Scrapers.Interfaces
{
    public interface IScraperService
    {
        Task<IEnumerable<DishDto>> ScrapeAsync(string entryPointUrl);
    }
}
