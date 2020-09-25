using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Radu.FoodScraper.Scrapers.Dto;
using Radu.FoodScraper.Scrapers.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Radu.FoodScraper.Scrapers
{
    public abstract class ScraperService : IScraperService
    {
        protected ILogger Logger { get; }
        public ScraperService(ILogger<ScraperService> logger)
        {
            Logger = logger;
        }

        public async Task<IEnumerable<DishDto>> ScrapeAsync(string entryPointUrl)
        {
            var menuLinks = await ExtractMenuLinksAsync(entryPointUrl);

            var dishes = new ConcurrentBag<DishDto>();

            var menuParseTasks = new List<Task>();
            foreach (var menuLink in menuLinks)
            {
                menuParseTasks.Add(Task.Run(() => ParseMenuPageAsync(menuLink, dishes)));
            }
            Task.WaitAll(menuParseTasks.ToArray());
            return dishes.ToList();
        }

        #region Protected
        protected ConcurrentDictionary<string, HtmlDocument> _pagesVisited = new ConcurrentDictionary<string, HtmlDocument>();
        protected async Task<HtmlDocument> GetHtmlAsync(string url)
        {
            if (_pagesVisited.ContainsKey(url.ToLower()))
            {
                Logger.LogDebug($"Getting content of {url} from cache.");
                return _pagesVisited[url.ToLower()];
            }
            else
            {
                Logger.LogDebug($"Getting content of {url}.");
                var document = new HtmlDocument();
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var content = await httpClient.GetStringAsync(url);
                        document.LoadHtml(content);
                        _pagesVisited.TryAdd(url.ToLower(), document);
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex, $"Network error while trying to retrieve {url}.");
                }

                return document;
            }
        }

        protected string ConstructUrl(string baseUrl, string relativeOrAbsolute)
        {
            return new Uri(new Uri(baseUrl), relativeOrAbsolute).ToString();
        }
        #endregion

        #region Abstract
        protected abstract Task<IEnumerable<string>> ExtractMenuLinksAsync(string entryPointUrl);
        protected abstract Task ParseMenuPageAsync(string menuPageUrl, ConcurrentBag<DishDto> dishes);
        #endregion
    }
}
