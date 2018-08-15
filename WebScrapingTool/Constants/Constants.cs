namespace WebScrapingTool.Constants
{   //This class will provide the application with constant values
    //Unit tests will also use a number of these constants
    public static class Constants
    {
        public static class ConfigOptions
        {
            public const string MonikerUriMapping = "MonikerUriMapping";
            public const string InteractionMode = "InteractionMode";
            public const string ConsoleRemainOpen = "ConsoleRemainOpen";
        }


        public static class UserInputProcessorErrorMessage
        {
            public const string UserInputInvalid = "Ensure User Input Is Valid";
            public const string MonikaArgumentNotDetected = "Moniker Argument Not Detected";
            public const string InvalidArgumentDetected = "Invalid Trailing Argument Detected";
            public const string DuplicateArgumentDetected = "Duplicate Trailing Argument Detected";
        }


        public static class ConfigManipulatorErrorMessage
        {
            public const string EnsureMonikerUriMappingInConfigFile = "Ensure MonikerUriMapping Is Present In The Config File";
            public const string UnableToParseConsoleRemainOpenOption = "Unable To Parse ConsoleRemainOpen Config Option";
            public const string UnableToParseInteractionModeOption = "Unable To Parse InteractionMode Config Option";
            public const string MonikerUriMappingInvalidFormat = "Ensure MonikerUriMapping Is Formatted Properly In Config File";
            public const string IssueLodingConfigFile = "Issue Loading Configuration File";
            public const string IssueDuringLoadingMapping = "Issue While Loading Mapping";
            public const string InvalidMonikerUriMappingItem = "Invalid MonikerUriMapping Item Detected";
            public const string DuplicateMappingItemDetected = "DuplicateMonikerUriMappingItemDetected";

            //DuplicateMonikerUriMappingItemDetected
        }


        public static class ApplicationCommand
        {
            public const string Exit = "#EXIT#";
        }


        public static class EncodingBom
        {
            public const string Utf8Bom = "ï»¿";
        }


        public static class FileType
        {
            public const string EXE = ".exe";
        }


        public static class Argument
        {
            public const string ArgumentIdentifier = "--";
        }


        public static class Separators
        {
            public const string SpaceSeparator = " ";
        }


        public static class MonikerUriMappingProperty
        {
            public const string MonikerName = "MonikerName";
            public const string Uri = "Uri";
            public const string ScraperType = "ScraperType";
        }


        public enum ScraperType
        {
            Unidentified = 0,
            HackerNewsScraper = 1
        };
    }
}

