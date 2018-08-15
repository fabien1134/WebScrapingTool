-How to run it (don't assume anything already installed)
---------------------------------------------------------
-Ensure .Net 4.7.2 developer pack and runtimes are installed

-Ensure the required configuration options are present in the configuration file
Config.json Options:
#ConsoleRemainOpen
When set to true,once the application has finished executing the console will remain open until input is provided.
When set to false the console will not remain open after executing.

#InteractionMode
If true once input has been provided, 
the web scraping execution will start asycrously then await for more input to be provided to start another webscraping task
To exit interaction mode the exit command should be provided as input then the application will wait until all webscraping task instances have completed

#MonikerUriMapping
MonikerURIMapping items will allow the application to know when an argument moniker value is entered, what uri should be used and what type of webscraper to use
e.g 
When 'hackernews --posts 5' is input the application will know hackernews is valid and what uri and scraper to use

MonikerUriMapping item:
{
      "MonikerName": "hackernews",
      "Uri": "https://news.ycombinator.com/news?p=1",
      "ScraperType": "HackerNewsScraper"
}

-Either user the OS CLI to run the application with arguements or start the EXE and provide arguments

-What libraries you used and why
---------------------------------
JSON.NET - Newtonsoft.Json library was used to interact and manipulate json.
The main reason is it provides a number of ways to serialize and deserialize json.
It provides a number of data structures with different interfaces that allows simplified json manipulation in effect reducing code
If the dll is required the nuget package manager command is:
Install-Package Newtonsoft.Json -Version 11.0.2
--
HtmlAgilityPack has been used to parse html as there .Net XML parsers had issues parsing the sites html due to invalid characters
To install open the nuget package manager console
input the following command: Install-Package HtmlAgilityPack -Version 1.8.6

-Additional Notes
---------------------------
--Due to time constraints the code quality of the HackerNewsScraper is reduced, I will optomise class using recursive functions
-Due to time constraints the unit tests for the HackerNewsScraper have not been implemented
--Did not include tracesourcing to log the stack during errors due to time constraints
--Assuming all trailing arguments will have a '--' Identifier
--Assumes a value will only appear after a trailing arguement
--Will have to becareful if the webclient timeout is increased as the application will wait until all tasks are complete
--Would usually have a tracesource log to store more detailed execution issues such as the exception stack trace
--Due to time limitation a canceltation token could not be included to signal to executing web scraping task instances when they should be forcedt to stop