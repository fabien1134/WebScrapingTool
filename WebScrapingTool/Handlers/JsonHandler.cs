using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace WebScrapingTool.Handlers
{   //This class is responsible for providing the application the ability to handle JSON
    public class JsonHandler
    {   //Will return a json file as a Json object
        public static JObject JsonFileToJobject(string filePath)
        {
            JObject configurationFileInstance = default;
            try
            {
                configurationFileInstance = JObject.Parse(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                throw new Exception($"Issue Converting JSON file To a JObject: {ex.Message}");
            }
            return configurationFileInstance;
        }

        //Will return a json collection as a type string list
        public static List<string> JsonToStringList(string json)
        {
            List<string> newList = new List<string>();

            JArray jsonArrayInstance = JArray.Parse(json);

            foreach (string arrayItem in jsonArrayInstance)
            {
                newList.Add(arrayItem);
            }

            return newList;
        }
    }
}
