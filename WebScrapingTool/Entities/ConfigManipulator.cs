using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using WebScrapingTool.Handlers;
using static WebScrapingTool.Constants.Constants;

namespace WebScrapingTool.Entities
{
    //This class is responsible for processing the configuration file and loading the moniker uri mapping dictionary
    public class ConfigManipulator
    {
        #region Properties
        //Will store values provided by the configuration file
        public bool InteractionMode { get; set; }
        public bool ConsoleRemainOpen { get; set; }
        #endregion

        #region Member Variables
        //Will store of a JObject containing values from the configuration file
        private JObject m_configurationInstance = default;
        #endregion


        //ToDo - Look into mocking dependency injection for testing purposes
        //Will be provided with the path to the configuration file or a configuration file json contents
        public ConfigManipulator(string configurationFilePath, string configurationFileJson = default)
        {
            m_configurationInstance = LoadConfigurationInstance(configurationFilePath, configurationFileJson);
            ParseConfigOptions(m_configurationInstance);
        }

        //Will load a dictionary that will associate moniker arguments with its uri and web scraping type it will use
        public Dictionary<string, (string Uri, ScraperType ScraperType)> LoadMonikerUriMappingDictionary()
        {
            //Ensure MonikerURI mapping property is present in the configuration file
            if (!m_configurationInstance.TryGetValue(ConfigOptions.MonikerUriMapping, out _))
                throw new Exception(ConfigManipulatorErrorMessage.EnsureMonikerUriMappingInConfigFile);

            Dictionary<string, (string Uri, ScraperType ScraperType)> monikerUriMapping = new Dictionary<string, (string, ScraperType)>();

            //Collect the collection of moniker uri mapping items
            JArray jsonNameAndUriMappingCollection = JArray.FromObject(m_configurationInstance[ConfigOptions.MonikerUriMapping]);

            //ToDo - Load moniker uri mapping item properties generically from a property setting
            string[] monikerUriMappingProperties = new string[] { MonikerUriMappingProperty.MonikerName, MonikerUriMappingProperty.Uri, MonikerUriMappingProperty.ScraperType };

            foreach (JToken mappingItem in jsonNameAndUriMappingCollection)
            {   //Change JToken mapping item to a JObject to provide simplified access to its properties
                JObject mappingItemObject = JObject.FromObject(mappingItem);

                //Validate the MonikerUriItem structure is valid - if any test fails then the property will not be valid
                foreach (string propertyName in monikerUriMappingProperties)
                {
                    bool validProperty = true;
                    //A property name should be present in the configuration file
                    validProperty = (mappingItemObject.TryGetValue(propertyName, out JToken parsedProperty)) ? validProperty : false;
                    //There should be a value for the property
                    validProperty = (!string.IsNullOrEmpty(parsedProperty?.ToString())) ? validProperty : false;

                    if (!validProperty)
                        throw new Exception(ConfigManipulatorErrorMessage.InvalidMonikerUriMappingItem);
                }

                ScraperType selectedScraperType = default;

                //Try to see if the value of the ScraperType is identified by the application
                if (!Enum.TryParse(mappingItemObject[MonikerUriMappingProperty.ScraperType].ToString(), ignoreCase: true, out ScraperType parsedScraperType))
                    selectedScraperType = ScraperType.Unidentified;

                //If the provided scraper type is not unidentified then update the selectedScraper type value
                if (parsedScraperType != ScraperType.Unidentified)
                    selectedScraperType = parsedScraperType;

                //The provided moniker name must be unique
                if (monikerUriMapping.ContainsKey(mappingItemObject[MonikerUriMappingProperty.MonikerName].ToString()))
                    throw new Exception(ConfigManipulatorErrorMessage.DuplicateMappingItemDetected);

                //Store the new valid and unique moniker uri mapping
                monikerUriMapping.Add(mappingItemObject[MonikerUriMappingProperty.MonikerName].ToString(), (mappingItemObject[MonikerUriMappingProperty.Uri].ToString(), selectedScraperType));
            }

            //Valid moniker uri mappings are expected by the application
            if (monikerUriMapping.Count == 0)
                throw new Exception(ConfigManipulatorErrorMessage.MonikerUriMappingInvalidFormat);

            return monikerUriMapping;
        }


        //This method will attempt to store additional selections found in the configuration file
        private void ParseConfigOptions(JObject configurationInstance)
        {   //Validate and try to parse the InteractionMode and ConsoleRemainOpen properties in the configuration file
            if (!bool.TryParse(configurationInstance[ConfigOptions.InteractionMode]?.ToString(), out bool interactionMode))
                throw new Exception(ConfigManipulatorErrorMessage.UnableToParseInteractionModeOption);

            InteractionMode = interactionMode;

            if (!bool.TryParse(configurationInstance[ConfigOptions.ConsoleRemainOpen]?.ToString(), out bool consoleRemainOpen))
                throw new Exception(ConfigManipulatorErrorMessage.UnableToParseConsoleRemainOpenOption);
            ConsoleRemainOpen = consoleRemainOpen;
        }

        //This method will decide what source should be used to load the configuration file instance
        private JObject LoadConfigurationInstance(string configurationFilePath, string configurationFileJson = default)
        {
            JObject configurationInstance = default;
            try
            {
                configurationInstance = (string.IsNullOrEmpty(configurationFileJson)) ? JsonHandler.JsonFileToJobject(configurationFilePath) : JObject.Parse(configurationFileJson);
            }
            catch (Exception)
            {
                throw new Exception(ConfigManipulatorErrorMessage.IssueLodingConfigFile);
            }

            return configurationInstance;
        }
    }
}
