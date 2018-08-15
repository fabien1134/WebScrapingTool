using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebScrapingTool.Interfaces
{   //Provides an interface for all web scraper
    public interface IWebScraper
    {
        Task ExecuteScraping(string uri, HttpClient httpClient, Dictionary<string, string> trailingArguments);
    }
}
