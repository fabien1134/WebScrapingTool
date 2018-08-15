using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebScrapingTool.Entities;
using WebScrapingTool.Properties;
using static WebScrapingTool.Constants.Constants;

namespace WebScrapingTool
{
    //This class contains the main program execution program flow
    class Program
    {
        static async Task Main(string[] args)
        {   //Http client will be used due provided simplified interface to make async requests
            HttpClient httpClient = new HttpClient();
            //Will store webscraping process instances
            List<Task> scrapingExecutions = new List<Task>();
            const string errorMessagePrefix = "Issue During Execution:";
            //Used to decide if the console should remain open post execution
            bool consoleRemainOpen = false;
            string input = default;

            try
            {
                Console.WriteLine($"Start Of Application Execution: {DateTime.Now}");

                //ToDo - Ensure a default configuration file is created in the required directory if it is not present
                //The configuration manipulator that will interact with the configuration file
                ConfigManipulator configManipulator = new ConfigManipulator(configurationFilePath: $@"{Directory.GetCurrentDirectory()}\{AppSettings.Default.ConfigFilePath}");
                //A mapping that associates a moniker, uri and the webscraper type that should be used
                Dictionary<string, (string Uri, ScraperType ScraperType)> monikerUriMapping = configManipulator.LoadMonikerUriMappingDictionary();

                //If arguments have been provided via the command line convert them to a string
                if (args.Length >= 1)
                    input = string.Join(Separators.SpaceSeparator, args);

                //Load additional options provided by the configuration file
                bool interactionMode = configManipulator.InteractionMode;
                consoleRemainOpen = configManipulator.ConsoleRemainOpen;

                //Instantiate user input processor to ensure input is valid
                UserInputProcessor userInputProcessor = new UserInputProcessor();

                //Will run at least once or continuously if interaction mode is true
                for (int i = 0; i < 1 || interactionMode; i++)
                {
                    try
                    {
                        //Only read from the console if no CLI arguments have been provided
                        if (string.IsNullOrEmpty(input))
                        {
                            Console.WriteLine("Enter Argument:");
                            input = Console.ReadLine();
                        }


                        //Validate and process input
                        userInputProcessor.ProcessInput(input);
                        //ToDo-- trigger cancellation token if it is able to be canceled to signal tasks instances they should stop
                        //Break out of the loop if the exit command has been input
                        if (userInputProcessor.Exit)
                            break;

                        //Throw an exception if the input is not valid
                        if (!userInputProcessor.IsUserInputValid)
                            throw new Exception($"Ensure Input Is Valid: {input}");

                        //Start a new scraping execution task instance if the input is valid
                        scrapingExecutions.Add(Task.Run(async () =>
                       {
                           try
                           {
                               //Ensures the moniker argument is valid e.g hackernews
                               if (!monikerUriMapping.Keys.Contains(userInputProcessor.MonikerArgument))
                                   throw new Exception("Moniker Not Found In The Config Files Moniker Uri Mapping Property");

                               //Collect its associated uri to provide the webscraper
                               string uri = monikerUriMapping[userInputProcessor.MonikerArgument].Uri;

                               //Select the associated scraper type that should be used with the valid maniker argument
                               switch (monikerUriMapping[userInputProcessor.MonikerArgument].ScraperType)
                               {
                                   case ScraperType.HackerNewsScraper:
                                       //Initialize algorithm based on selected mapping item 
                                       try
                                       {
                                           //Request body
                                           string responseBody = default;
                                           using (HttpResponseMessage responseMessage = await httpClient.GetAsync(uri))
                                           using (HttpContent content = responseMessage.Content)
                                           {
                                               responseBody = await content.ReadAsStringAsync();
                                           }

                                           Console.WriteLine(responseBody);
                                       }
                                       catch (Exception ex)
                                       {
                                           //Display error information based on web scrapping execution errors
                                           //Log Error Message
                                       }
                                       break;
                                   default://Will throw an exception if the associated web scraper is unidentified
                                       throw new Exception("Provided Web Scraper Is Unidentified");
                               }
                           }
                           catch (Exception ex)
                           {
                               //Display error information based on web scrapping execution errors
                               //Log Error Message
                               Console.WriteLine($"Issue During {userInputProcessor.MonikerArgument} Process: {ex.Message}");
                           }
                       }));
                    }
                    catch (Exception ex)
                    {
                        //Ensures application can continue when running in interaction mode
                        //If application was extended to run a collection of inputs, ensures collection items can be enumerated if errors occurs during process
                        Console.WriteLine($"{errorMessagePrefix} {ex.Message}");
                    }

                    //Clear input
                    input = string.Empty;
                }
            }
            catch (Exception ex)
            {   //Display error message that caused main execution to stop
                Console.WriteLine($"{errorMessagePrefix} {ex.Message}");
            }
            finally
            {   //Wait for scraping executions to complete
                if (scrapingExecutions.Count() >= 1)
                    await Task.WhenAll(scrapingExecutions);

                httpClient.Dispose();
            }

            Console.WriteLine($"End Of Application Execution: {DateTime.Now}");
            //Temp Will Allow The Viewing Of The Results
            if (consoleRemainOpen)
                Console.ReadLine();
        }
    }
}
