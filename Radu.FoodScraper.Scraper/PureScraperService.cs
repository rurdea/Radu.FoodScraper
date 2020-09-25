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

        protected override async Task<IEnumerable<string>> ExtractMenuLinksAsync(string url)
        {
            try
            {   // this will only work with a menu page as entry point eg: https://www.pure.co.uk/menus/breakfast
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

                return menus.Select(n => ConstructUrl(url, n.Attributes["href"].Value));
            }
            catch (Exception)
            {   // to do: log
                return null;
            }

        PAGE_CHANGED:
            // to do: log page changed warning
            return null;
        }

        protected override async Task ParseMenuPageAsync(string menuLink, ConcurrentBag<DishDto> dishes)
        {
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
                if (menuTitles == null) goto PAGE_CHANGED;

                foreach (var menuTitle in menuTitles)
                {
                    try
                    {
                        // extract section title information
                        var menuTitleText = menuTitle.SelectSingleNode(".//span")?.InnerText;
                        var menuTitleId = menuTitle.Attributes["aria-controls"].Value;

                        // find all dish title nodes
                        var dishTitles = document.DocumentNode.SelectSingleNode($"//div[@id='{menuTitleId}']")?.SelectNodes($".//div[contains(@class, '{CSS_CLASS_DISH_TITLE}')]/a");
                        if (dishTitles == null) continue;
                        foreach (var dishTitle in dishTitles)
                        {
                            dishes.Add(new DishDto()
                            {
                                MenuTitle = menuName,
                                MenuDescription = menuDescription,
                                // to do: create a wrapper around dish dto with a task member that will allow parallel description extractions
                                DishDescription = await ExtractDishDescriptionAsync(ConstructUrl(menuLink, dishTitle.Attributes["href"].Value)),
                                DishName = dishTitle.Attributes["title"].Value,
                                MenuSectionTitle = menuTitleText
                            });
                        }
                    }
                    catch(Exception ex)
                    {
                        //log warning and continue;
                    }
                }
            }
            catch (Exception)
            {   // to do: log
            }
        PAGE_CHANGED:
            // to do: log page changed warning
            return;
        }

        private async Task<string> ExtractDishDescriptionAsync(string dishLink)
        {
            try
            {
                var document = await GetHtmlAsync(dishLink);
                return document.DocumentNode.SelectSingleNode($"//article[@class='{CSS_CLASS_DISH_DETAILS}']/div/p")?.InnerText;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

