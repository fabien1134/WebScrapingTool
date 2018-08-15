using static WebScrapingTool.Constants.Constants;

namespace WebScrapingTool.Interfaces
{   //Abstract factory interface for create different types of scrapers
    public interface IScraperFactory//interface
    {
         IWebScraper CreateWebScraper(ScraperType scraperType);
    }
}
