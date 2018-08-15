using System;
using WebScrapingTool.Interfaces;
using static WebScrapingTool.Constants.Constants;

namespace WebScrapingTool.Entities
{
    //Concrete factory responsible for creating scrapers
    public class ScraperFactory : IScraperFactory
    {
        public static IWebScraper CreateWebScraperInstance(ScraperType scraperType) => new ScraperFactory().CreateWebScraper(scraperType);

        public IWebScraper CreateWebScraper(ScraperType scraperType)
        {
            IWebScraper newWebScraperInstance = default;
            switch (scraperType)
            {
                case ScraperType.Unidentified:
                    throw new Exception("Scraper Type Must Be Identified");
                case ScraperType.HackerNewsScraper:
                    newWebScraperInstance = new HackerNewsScraper();
                    break;
            }

            return newWebScraperInstance;
        }
    }
}
