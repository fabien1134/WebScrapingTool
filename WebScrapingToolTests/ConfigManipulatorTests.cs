using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WebScrapingTool.Entities;
using static WebScrapingTool.Constants.Constants;

namespace WebScrapingToolTests
{
    [TestClass]
    public class ConfigManipulatorTests
    {
        [DataTestMethod]
        //An argument may not always require a value
        [DataRow(@"{
  ""ConsoleRemainOpen"": true,
  ""InteractionMode"": true
}"
            , true, true)]

        [DataRow(@"{
  ""ConsoleRemainOpen"": false,
  ""InteractionMode"": false
}"
            , false, false)]
        public void TestConfigManipulator_ExpectedStateAchieved(string json, bool ConsoleRemainOpenExpectedValue, bool InteractionModeExpectedValue)
        {
            //Arrange
            string execeptionMessage = string.Empty;

            //Act 
            ConfigManipulator configManipulator = new ConfigManipulator(configurationFilePath: string.Empty, configurationFileJson: json);

            //Assert
            Assert.IsTrue(configManipulator.ConsoleRemainOpen == ConsoleRemainOpenExpectedValue && configManipulator.InteractionMode == InteractionModeExpectedValue);
        }

        [DataTestMethod]
        [DataRow("", ConfigManipulatorErrorMessage.IssueLodingConfigFile)]
        [DataRow(@"{""ConsoleRemainOpen"": false}", ConfigManipulatorErrorMessage.UnableToParseInteractionModeOption)]
        [DataRow(@"{""InteractionMode"": false}", ConfigManipulatorErrorMessage.UnableToParseConsoleRemainOpenOption)]
        [DataRow(@"{
  ""ConsoleRemainOpen"": false,
  ""InteractionMode"": false,
  ""MonikerUriMapping"": [ {  ""MonikerName"": ""hackernews""  }]}"
            , ConfigManipulatorErrorMessage.InvalidMonikerUriMappingItem)]
        [DataRow(@"{
  ""ConsoleRemainOpen"": false,
  ""InteractionMode"": false,
  ""MonikerUriMapping"": [
    {
      ""MonikerName"": ""hackernews"",
      ""Uri"": ""https://news.ycombinator.com/news?p=1"",
      ""ScraperType"": """" }]}"
            , ConfigManipulatorErrorMessage.InvalidMonikerUriMappingItem)]
        [DataRow(@"{
  ""ConsoleRemainOpen"": false,
  ""InteractionMode"": false,
  ""MonikerUriMapping"": [
    {
      ""MonikerName"": ""hackernews"",
      ""Uri"": ""https://news.ycombinator.com/news?p=1"",
      ""ScraperType"": ""HackerNewsScraper"" },
    {
      ""MonikerName"": ""hackernews"",
      ""Uri"": ""https://news.ycombinator.com/news?p=3"",
      ""ScraperType"": ""FacebookScraper"" }]}"
            , ConfigManipulatorErrorMessage.DuplicateMappingItemDetected)]
        [DataRow(@"{
  ""ConsoleRemainOpen"": false,
  ""InteractionMode"": false}"
            , ConfigManipulatorErrorMessage.EnsureMonikerUriMappingInConfigFile)]
        public void TestConfigManipulator_InvalidConfigHandled(string json, string expextedErrorMessage)
        {
            //Arrange
            string execeptionMessage = string.Empty;

            //Act 
            try
            {
                ConfigManipulator configManipulator = new ConfigManipulator(configurationFilePath: string.Empty, configurationFileJson: json);
                var monikerUriMappingDictionary = configManipulator.LoadMonikerUriMappingDictionary();
            }
            catch (Exception ex)
            {
                execeptionMessage = ex.Message;
            }

            //Assert
            Assert.IsTrue(string.Compare(expextedErrorMessage, execeptionMessage) == 0);
        }
    }
}
