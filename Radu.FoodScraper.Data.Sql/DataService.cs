using Radu.FoodScraper.Data.Interfaces;
using Radu.FoodScraper.Models;
using System;
using System.Threading.Tasks;

namespace Radu.FoodScraper.Data.Sql
{
    public class DataService : IDataService
    {
        public DataService(string connectionString)
        {
            
        }

        public async Task PersistAsync(string scraper, System.Collections.Generic.IEnumerable<Dish> dishes)
        {
            await Task.Delay(2000);
            throw new Exception("test");
            await Task.FromResult(0);
        }
    }
}
