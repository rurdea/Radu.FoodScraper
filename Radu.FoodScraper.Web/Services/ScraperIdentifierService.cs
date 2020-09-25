using Radu.FoodScraper.Scrapers;
using Radu.FoodScraper.Scrapers.Interfaces;
using System.Threading.Tasks;

namespace Radu.FoodScraper.Web.Services
{
    /// <summary>
    /// Service class used to identity the scraper service based on the entry url.
    /// For now it will return the PureScraperService by default as it is the only scraper implemented for this challenge.
    /// </summary>
    public class ScraperIdentifierService
    {
        public async Task<IScraperService> GetScraperAsync(string url)
        {
            // to do: read configured scrapers from database and match with the url
            // hardcode to pure scraper for this challenge
            return await Task.FromResult(new PureScraperService());
        }
    }
}
