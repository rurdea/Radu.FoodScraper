using Radu.FoodScraper.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Radu.FoodScraper.Data.Interfaces
{
    public interface IDataService
    {
        Task PersistAsync(string scraper, IEnumerable<Dish> dishes);
    }
}
