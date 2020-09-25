using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Radu.FoodScraper.Data.Sql;
using Radu.FoodScraper.Models;
using Radu.FoodScraper.Scrapers.Dto;
using Radu.FoodScraper.Web.InputModels;
using Radu.FoodScraper.Web.Services;

namespace Radu.PureScraper.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapeController : ControllerBase
    {
        private readonly ILogger<ScrapeController> _logger;
        private readonly ScraperIdentifierService _scraperIdentifier;
        private readonly MapperService _mapperService;
        private readonly DataService _dataService;

        public ScrapeController(ILogger<ScrapeController> logger, 
                                ScraperIdentifierService scraperIdentifier,
                                MapperService mapperService,
                                DataService dataService)
        {
            _logger = logger;
            _scraperIdentifier = scraperIdentifier;
            _mapperService = mapperService;
            _dataService = dataService;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<DishDto>>> Post([FromBody]ScrapeInputModel input)
        {
            if (string.IsNullOrWhiteSpace(input?.MenuUrl)) return BadRequest("MenuUrl not provided.");
            _logger.LogInformation("test");
            // get the scraper based on the url
            var scraperService = await _scraperIdentifier.GetScraperAsync(input.MenuUrl);

            var dishes = await scraperService.ScrapeAsync(input.MenuUrl);

            if (dishes==null || dishes.Count() == 0)
            { 
                return Ok();
            }

            var models = dishes.Select(d => _mapperService.Map<Dish>(d));
            
            // persist dishes in the database async
            Task.Run(() => _dataService.PersistAsync(scraperService.GetType().Name, models));

            return Ok(dishes);
        }
    }
}
