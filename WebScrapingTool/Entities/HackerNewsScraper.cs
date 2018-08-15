using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebScrapingTool.Interfaces;

namespace WebScrapingTool.Entities
{
    //This is the concrete webscraper used to analyze and process data from hacker news
    public class HackerNewsScraper : IWebScraper
    {
        #region Member Variables
        //Will be used to make asynchronous requests
        private HttpClient m_httpClient = default;
        private const string m_expectedArgument = "posts";
        private const string m_invalidInputErrorMessage = "HackerNews Scraper Input Is Invalid";
        #endregion

        public HackerNewsScraper() { }

        public async Task ExecuteScraping(string uri, HttpClient httpClient, Dictionary<string, string> trailingArguments)
        {
            //Check if the correct trailing arguments have been provided
            ContinueIfInputIsValid(uri, httpClient, trailingArguments);
            m_httpClient = httpClient;
            string responseBody = await RequestFromTarget(uri);
            //Console.WriteLine(responseBody);






        }


        private void ContinueIfInputIsValid(string uri, HttpClient httpClient, Dictionary<string, string> trailingArguments)
        {
            //Check if the correct trailing arguments have been provided
            if (string.IsNullOrEmpty(uri) || httpClient == null || trailingArguments == null)
                throw new Exception(m_invalidInputErrorMessage);

            if (!trailingArguments.ContainsKey($"{m_expectedArgument}"))
                throw new Exception(m_invalidInputErrorMessage);

            if (!int.TryParse(trailingArguments[m_expectedArgument], out int parsedPostAmountValue))
                throw new Exception(m_invalidInputErrorMessage);

            if (!(parsedPostAmountValue >= 1 && parsedPostAmountValue <= 100))
                throw new Exception(m_invalidInputErrorMessage);

        }

        private async Task<string> RequestFromTarget(string uri)
        {
            //Request body
            string responseBody = default;
            using (HttpResponseMessage responseMessage = await m_httpClient.GetAsync(uri))
            using (HttpContent content = responseMessage.Content)
            {
                responseBody = await content.ReadAsStringAsync();
            }

            return responseBody;
        }
    }
}
