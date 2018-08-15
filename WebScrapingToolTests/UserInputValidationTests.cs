using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebScrapingTool.Entities;
using static WebScrapingTool.Constants.Constants;

namespace WebScrapingToolTests
{
    [TestClass]
    public class UserInputValidationTests
    {
        [DataTestMethod]
        //An argument may not always require a value
        [DataRow("hackernews --posts", true)]
        [DataRow("hackernews --posts 5", true)]
        //Trailing arguments may not always be required
        [DataRow("hackernews", true)]
        [DataRow(ApplicationCommand.Exit, false)]

        public void TestUserInputProcessor_ValidInputHandling(string userInput, bool expectedUserInputProcessorState)
        {
            //Arrange
            string execeptionMessage = string.Empty;
            UserInputProcessor userInputProcessor = new UserInputProcessor();

            //Act 
            try
            {
                userInputProcessor.ProcessInput(userInput);
            }
            catch (System.Exception ex)
            {
                execeptionMessage = ex.Message;
            }


            //Assert
            Assert.IsTrue(expectedUserInputProcessorState == userInputProcessor.IsUserInputValid);
        }


        [DataTestMethod]
        [DataRow(ApplicationCommand.Exit, "", true)]
        [DataRow("hackernews --posts 5", "", false)]
        public void TestUserInputProcessor_ExitStateEffect(string userInput, string expectedErrorMessage, bool expectedExitState)
        {
            //Arrange
            string execeptionMessage = string.Empty;
            UserInputProcessor userInputProcessor = new UserInputProcessor();

            //Act 
            try
            {
                userInputProcessor.ProcessInput(userInput);
            }
            catch (System.Exception ex)
            {
                execeptionMessage = ex.Message;
            }

            //Assert
            Assert.IsTrue(expectedExitState == userInputProcessor.Exit && string.Compare(execeptionMessage, expectedErrorMessage) == 0);
        }


        [DataTestMethod]
        [DataRow("--hello 5 --posts 7", UserInputProcessorErrorMessage.MonikaArgumentNotDetected, false)]
        //Ensure A Monika argument is provided
        [DataRow(" ", UserInputProcessorErrorMessage.UserInputInvalid, false)]
        [DataRow("", UserInputProcessorErrorMessage.UserInputInvalid, false)]
        //Test Trailing Argument Issues
        [DataRow("hackernews   posts-- 5 ", UserInputProcessorErrorMessage.InvalidArgumentDetected, false)]
        [DataRow("hackernews --hello 5 --posts 5 ", UserInputProcessorErrorMessage.InvalidArgumentDetected, false)]
        [DataRow("hackernews --posts 5 --hello 5", UserInputProcessorErrorMessage.InvalidArgumentDetected, false)]
        [DataRow("hackernews --hello 5", UserInputProcessorErrorMessage.InvalidArgumentDetected, false)]
        //Test duplicate trailing argument issues
        [DataRow("hackernews --posts 5 --posts 7", UserInputProcessorErrorMessage.DuplicateArgumentDetected, false)]
        public void TestUserInputProcessor_InvalidInputHandling(string userInput, string expectedErrorMessage, bool expectedUserInputProcessorState)
        {
            //Arrange
            string execeptionMessage = string.Empty;
            UserInputProcessor userInputProcessor = new UserInputProcessor();

            //Act 
            try
            {
                userInputProcessor.ProcessInput(userInput);
            }
            catch (System.Exception ex)
            {
                execeptionMessage = ex.Message;
            }

            //Assert
            Assert.IsTrue(expectedUserInputProcessorState == userInputProcessor.IsUserInputValid && string.Compare(execeptionMessage, expectedErrorMessage) == 0);
        }
    }
}
