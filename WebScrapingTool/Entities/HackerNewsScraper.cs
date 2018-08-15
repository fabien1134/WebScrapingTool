using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
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
            ContinueIfInputIsValid(uri, httpClient, trailingArguments, out int parsedPostAmountValue);
            m_httpClient = httpClient;

            //Will keep track of how many valid HackerRankPosts have been processed and added to the collection
            int validHackerRankPostsProcessed = 0;
            List<HackerNewsPost> hackerNewsPostCollection = new List<HackerNewsPost>();
            int hackerRankPostTablePosition = 0;
            //Will decide if the application will have to paginate
            bool stopHackerPostDownloads = false;
            string previousPaginationTarget = string.Empty;
            string paginationTargetSuffix = string.Empty;
            bool paginatorLinkFound = false;

            while (!stopHackerPostDownloads)
            {
                //Reset state for following page link finding task
                paginatorLinkFound = false;
                hackerRankPostTablePosition = 0;
                //If previous next uri is the same as the current one then pagination cannot continue
                if (!stopHackerPostDownloads)
                {
                    if (previousPaginationTarget == $"{uri}{paginationTargetSuffix}")
                    {
                        Console.WriteLine("Stopping Hacker Post Download, No New URI Pagination Target Provided");
                        break;
                    }
                }
                else
                {   //Exit Posts processor loop
                    break;
                }

                //Make an async request
                string responseBody = await RequestFromTarget($"{uri}{paginationTargetSuffix}");
                //Store the current uri target as the previous one
                previousPaginationTarget = $"{uri}{paginationTargetSuffix}";

                //Parse returned HTML
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(responseBody);

                //Return nodes with the required data that must be extracted
                HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectSingleNode(@"/html[1]/body[1]/center[1]/table[1]/tr[3]/td[1]/table[1]").ChildNodes;
                HackerNewsPost hackerNewsPost = default;
                //Iterate through all childnodes that contains data we are interested in then collect the data if nodes are valid
                foreach (HtmlNode htmlNode in htmlNodes)
                {   //Only create a new item when starting from 0
                    if (hackerRankPostTablePosition == 0)
                        hackerNewsPost = new HackerNewsPost();

                    //Will be used to see if we are on the first or last table row belonging to a hacker news post

                    if (string.Compare(htmlNode.Name, "tr") == 0)
                    {
                        //If the pagination link is detected store it then exit the loop
                        if (string.Compare(htmlNode.InnerText, "More") == 0)
                        {
                            paginatorLinkFound = true;
                            //Collect the pagination link
                            paginationTargetSuffix = htmlNode.SelectSingleNode(@"/html[1]/body[1]/center[1]/table[1]/tr[3]/td[1]/table[1]/tr[92]/td[2]/a[1]/@href[1]").GetAttributeValue("href", string.Empty);
                            break;
                        }

                        //Collect appropriate data
                        if (hackerRankPostTablePosition == 0)
                        {
                            //Get rank
                            hackerNewsPost.rank = (int.TryParse(htmlNode.ChildNodes[1].ChildNodes[0].InnerText.ToString().Replace(".", string.Empty).Trim(), out int parsedPoints)) ? parsedPoints : -1;
                            hackerNewsPost.title = htmlNode.ChildNodes[4].ChildNodes[0].InnerText.ToString();
                            hackerNewsPost.uri = htmlNode.ChildNodes[4].ChildNodes[0].Attributes[0].Value.ToString();
                        }
                        else if (hackerRankPostTablePosition == 1)
                        {
                            hackerNewsPost.points = (int.TryParse(htmlNode.ChildNodes[1].ChildNodes[1].InnerText.Replace("points", string.Empty).Trim(), out int parsedPoints)) ? parsedPoints : -1;
                            hackerNewsPost.comments = (int.TryParse(htmlNode.ChildNodes[1].ChildNodes[11].InnerText.Replace("&nbsp;comments", string.Empty).Trim(), out int parsedComments)) ? parsedComments : -1;
                            hackerNewsPost.author = htmlNode.ChildNodes[1].ChildNodes[3].InnerText;
                        }

                        hackerRankPostTablePosition++;

                        //If a table row spacer is detected then reset the hackerRankPostTablePosition counter as we will be looking for the next post
                        //increment the validHackerRankPostsProcessed by one and break from the loop if the expected amount is reached
                        if (htmlNode.HasAttributes)
                            if (string.Compare(htmlNode.Attributes["class"].Value, "spacer") == 0)
                            {   //Only add new post item if this is valid
                                if (hackerNewsPost.IsValid)
                                {
                                    //Store hacker new post in our collection
                                    hackerNewsPostCollection.Add(hackerNewsPost);
                                    //Increment position once data row in a table row position has been processed
                                    validHackerRankPostsProcessed++;
                                    //Stop processing posts if we have reached the desired amount of processed posts
                                    stopHackerPostDownloads = validHackerRankPostsProcessed == parsedPostAmountValue;
                                    if (stopHackerPostDownloads)
                                    {
                                        Console.WriteLine("All Valid Hacker Rank Posts Downloaded");
                                        break;
                                    }
                                }

                                hackerRankPostTablePosition = 0;
                            }

                        //Ensures only two table rows hacker post items are processed
                        if (hackerRankPostTablePosition > 2)
                            throw new Exception("Maximum Rows To Be Processed Per HackRank Post Is Two");
                    }
                }

                //If no pagination link is found then pagination should stop
                if (!paginatorLinkFound && !stopHackerPostDownloads)
                {
                    stopHackerPostDownloads = true;
                    Console.WriteLine("No Pagination Link Provided, Cannot Download Any More Posts");
                }
            }


            ////Print out the results serialized results to JSON
            if (hackerNewsPostCollection.Count >= 1)
            {
                Console.WriteLine(JArray.FromObject(hackerNewsPostCollection).ToString());
            }
            else
            {
                Console.WriteLine("No HackerRank Posts To Output");
            }
        }


        private void ContinueIfInputIsValid(string uri, HttpClient httpClient, Dictionary<string, string> trailingArguments, out int parsedPostAmountValue)
        {
            parsedPostAmountValue = 0;
            //Check if the correct trailing arguments have been provided
            if (string.IsNullOrEmpty(uri) || httpClient == null || trailingArguments == null)
                throw new Exception(m_invalidInputErrorMessage);

            if (!trailingArguments.ContainsKey($"{m_expectedArgument}"))
                throw new Exception(m_invalidInputErrorMessage);

            if (!int.TryParse(trailingArguments[m_expectedArgument], out parsedPostAmountValue))
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
