using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Radu.FoodScraper.Scrapers.Dto;
using Radu.FoodScraper.Scrapers.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Radu.FoodScraper.Scrapers
{
    public class PureScraperService : ScraperService, IScraperService
    {
        #region Consts
        private const string CSS_CLASS_SUBMENU = "submenu";
        private const string CSS_CLASS_MENU_HEADER = "menu-header";
        private const string CSS_CLASS_MENU_TITLE = "menu-title";
        private const string CSS_CLASS_DISH_TITLE = "menu-item";
        private const string CSS_CLASS_DISH_DETAILS = "menu-item-details";
        #endregion

        public PureScraperService(ILogger<PureScraperService> logger) : base(logger)
        {
        }

        protected override async Task<IEnumerable<string>> ExtractMenuLinksAsync(string url)
        {
            try
            {
                Logger.LogDebug($"Extracting menu links from {url}.");
                // this will only work with a menu page as entry point eg: https://www.pure.co.uk/menus/breakfast
                // to do: if needed, parse pages like https://www.pure.co.uk/menus/ or https://www.pure.co.uk/ to extract the menu too
                var document = await GetHtmlAsync(url);

                var submenu = document.DocumentNode.SelectNodes($"//ul[@class='{CSS_CLASS_SUBMENU}']").FirstOrDefault();

                if (submenu == null)
                {
                    goto PAGE_CHANGED;
                }

                var menus = submenu.SelectNodes(".//a");
                if (menus == null)
                {
                    goto PAGE_CHANGED;
                }
                Logger.LogInformation("Menu links extracted successfully.");
                return menus.Select(n => ConstructUrl(url, n.Attributes["href"].Value));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error extracting menu links.");
            }
            return null;
        PAGE_CHANGED:
            Logger.LogWarning($"No menu links found at {url}, page souce might have been changed and the parser needs to be updated.");
            return null;
        }

        protected override async Task ParseMenuPageAsync(string menuLink, ConcurrentBag<DishDto> dishes)
        {
            Logger.LogDebug($"Parsing menu page at {menuLink}.");

            try
            {
                var document = await GetHtmlAsync(menuLink);
                // get the header element
                var header = document.DocumentNode.SelectSingleNode($"//header[@class='{CSS_CLASS_MENU_HEADER}']");
                if (header == null) goto PAGE_CHANGED;

                // extract menu information from the header
                var menuName = header.SelectSingleNode(".//h1 | .//h2")?.InnerText;
                var menuDescription = header.SelectSingleNode(".//p").InnerText.Replace("\n", " ");

                // find all section title nodes
                var menuTitles = document.DocumentNode.SelectNodes($"//h4[@class='{CSS_CLASS_MENU_TITLE}']/a");
                if (menuTitles == null)
                {
                    // no menu titles for pages "Welling Boxes" and "Salads", search the full page for dishes
                    var dishTitles = document.DocumentNode.SelectSingleNode($"//section[@class='container']")?.SelectNodes($".//div[contains(@class, '{CSS_CLASS_DISH_TITLE}')]/a");
                    await ParseDishTitles(dishTitles, menuLink, menuName, menuDescription, null, dishes);
                }
                else
                {
                    foreach (var menuTitle in menuTitles)
                    {
                        // extract section title information
                        var menuTitleText = menuTitle.SelectSingleNode(".//span")?.InnerText;
                        var menuTitleId = menuTitle.Attributes["aria-controls"]?.Value;
                        if (menuTitleId == null)
                        {
                            Logger.LogWarning($"Could not parse menu title {menuTitleText}, page source might have been changed and the parser needs to be updated");
                            // continue with other sections if one section does not have the correct html
                            continue;
                        }
                        else
                        {
                            // find all dish title nodes
                            var dishTitles = document.DocumentNode.SelectSingleNode($"//div[@id='{menuTitleId}']")?.SelectNodes($".//div[contains(@class, '{CSS_CLASS_DISH_TITLE}')]/a");
                            await ParseDishTitles(dishTitles, menuLink, menuName, menuDescription, menuTitleText, dishes);
                        }


                        Logger.LogInformation($"Successfully parsed dishes at menu title {menuTitleText}.");
                    }
                }

                Logger.LogInformation($"Successfully parsed menu page at {menuLink}.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error parsing menu page {menuLink}.");
            }
            return;
        PAGE_CHANGED:
            Logger.LogWarning($"No dish links found at {menuLink}, page souce might have been changed and the parser needs to be updated.");
            return;
        }

        private async Task ParseDishTitles(HtmlNodeCollection dishTitles, string menuLink, string menuName, string menuDescription, string menuTitleText, ConcurrentBag<DishDto> dishes)
        {
            if (dishTitles == null) return;
            var loadDescriptionTasks = new List<Task>();
            foreach (var dishTitle in dishTitles)
            {
                var dish = new DishDto()
                {
                    MenuTitle = menuName,
                    MenuDescription = menuDescription,
                    // to do: create a wrapper around dish dto with a task member that will allow parallel description extractions
                    //DishDescription = await ExtractDishDescriptionAsync(ConstructUrl(menuLink, dishTitle.Attributes["href"].Value)),
                    DishName = dishTitle.Attributes["title"].Value,
                    MenuSectionTitle = menuTitleText
                };

                loadDescriptionTasks.Add(Task.Run(async () => dish.DishDescription = await ExtractDishDescriptionAsync(ConstructUrl(menuLink, dishTitle.Attributes["href"].Value))));
                dishes.Add(dish);    
                Logger.LogInformation($"New dish {dish.DishName} created");
            }
            Task.WaitAll(loadDescriptionTasks.ToArray());
        }

        private async Task<string> ExtractDishDescriptionAsync(string dishLink)
        {
            try
            {
                var document = await GetHtmlAsync(dishLink);
                return document.DocumentNode.SelectSingleNode($"//article[@class='{CSS_CLASS_DISH_DETAILS}']/div/p")?.InnerText;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Error extracting dish description from {dishLink}");
                return null;
            }
        }
    }
}

