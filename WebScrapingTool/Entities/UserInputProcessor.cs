using System;
using System.Collections.Generic;
using System.Linq;
using WebScrapingTool.Handlers;
using static WebScrapingTool.Constants.Constants;

namespace WebScrapingTool.Entities
{
    //This class will be responsible for validating and processing the user input
    public class UserInputProcessor
    {
        #region Properties
        //Will let the application know if the user input is valid
        public bool IsUserInputValid { get; private set; }
        //Will store the value, the webscraper should target e.g hackernews
        public string MonikerArgument { get; private set; }
        //Will store all valid trailing arguments found in input e.g --posts 5
        public Dictionary<string, string> TrailingArguments { get; private set; } = new Dictionary<string, string>();
        //Will signal if an argument has been provided to exit the application
        public bool Exit { get; private set; }
        #endregion

        //The input could be provided to the constructor and processed
        public UserInputProcessor(string input = default)
        {
            if (!string.IsNullOrEmpty(input))
                ProcessInput(input);
        }

        //Will validate and process input extracting the moniker argument and trailing arguments
        public void ProcessInput(string input)
        {   //Ensure whitespace is removed
            input = input?.Trim();

            //Provided input cannot be null
            if (string.IsNullOrEmpty(input))
            {
                IsUserInputValid = false;
                throw new Exception(UserInputProcessorErrorMessage.UserInputInvalid);
            }

            //Reset State
            ResetState();

            //Ensure the application is signaled that it should stop execution
            if (input.Contains(ApplicationCommand.Exit))
            {
                Exit = true;
            }
            else
            {
                //Extract the moniker and trailing arguments 
                MonikerArgument = ExtractArguments(input);
            }
        }

        private string ExtractArguments(string input)
        {
            //Extract the moniker argument if it is present
            string monikerArgument = input.Split(new string[] { Separators.SpaceSeparator }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            //A moniker argument must be present
            if (!string.IsNullOrEmpty(monikerArgument))
            {
                IsUserInputValid = true;

                //The moniker argument cannot be a trailing argument or contain an argument identifier
                if (monikerArgument.Contains(Argument.ArgumentIdentifier))
                {
                    IsUserInputValid = false;
                    throw new Exception(UserInputProcessorErrorMessage.MonikaArgumentNotDetected);
                }

                //Store remaining trailing arguments
                string trailingArguments = input.Replace(monikerArgument, string.Empty).Trim();

                //ToDo - Ensure all trailing arguments contain an Argument Identifier as a prefix as 'posts' is currently valid when it should be false

                //Extract all trailing arguments if string is present
                if (!string.IsNullOrEmpty(trailingArguments) && trailingArguments.Contains(Separators.SpaceSeparator))
                    ExtractAndStoreTrailingArguments(trailingArguments.Split(new string[] { Argument.ArgumentIdentifier }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                throw new Exception(UserInputProcessorErrorMessage.MonikaArgumentNotDetected);
            }

            return monikerArgument;
        }

        //Extract and store valid trailing arguments
        private void ExtractAndStoreTrailingArguments(string[] trailingArguments)
        {   //Collect all valid trailing commands from an embedded json file
            //validTrailingCommands are valid trailing commands the application will accept
            string validTrailingCommandsResourceContents = EmbeddedResourceHandler.GetAssemblyResourceAsString($"{Properties.AppSettings.Default.ResourceManifestPath}{Properties.AppSettings.Default.ValidCommandsResourceFileName}");
            List<string> validTrailingCommands = JsonHandler.JsonToStringList(validTrailingCommandsResourceContents);

            //Validate and store all valid trailing arguments, all provided trailing arguments should be valid
            foreach (string trailingArgument in trailingArguments)
            {
                //Split and store the name and value of the trailing argument
                string trailingArgumentName = string.Empty;
                string trailingArgumentValue = string.Empty;

                string[] argumentValuePair = trailingArgument?.Split(new string[] { Separators.SpaceSeparator }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < argumentValuePair.Length; i++)
                {
                    if (i == 0)
                    {
                        trailingArgumentName = argumentValuePair[i]?.Trim();
                    }
                    else if (i == 1)
                    {
                        trailingArgumentValue = argumentValuePair[i]?.Trim();
                    }
                }

                //Check if this is a valid trailing argument
                if (string.IsNullOrEmpty(trailingArgumentName))
                    break;

                //Check if the provided trailing argument name is valid
                if (!validTrailingCommands.Contains(trailingArgumentName))
                {
                    IsUserInputValid = false;
                    throw new Exception(UserInputProcessorErrorMessage.InvalidArgumentDetected);
                }

                //Ensure no duplicate arguments are added
                if (TrailingArguments.Keys.Contains(trailingArgumentName))
                {
                    IsUserInputValid = false;
                    throw new Exception(UserInputProcessorErrorMessage.DuplicateArgumentDetected);
                }

                //Store valid trailing argument name and value pair
                TrailingArguments.Add(trailingArgumentName, trailingArgumentValue);
            }
        }

        //Ensures the state is reset
        private void ResetState()
        {
            Exit = false;
            IsUserInputValid = false;
            TrailingArguments = new Dictionary<string, string>();
            MonikerArgument = string.Empty;
        }
    }
}
