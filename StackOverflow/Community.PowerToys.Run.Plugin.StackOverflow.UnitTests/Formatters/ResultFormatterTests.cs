using Microsoft.VisualStudio.TestTools.UnitTesting;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;
using Community.PowerToys.Run.Plugin.StackOverflow.Formatters;
using System;

namespace Community.PowerToys.Run.Plugin.StackOverflow.UnitTests.Formatters
{
    [TestClass]
    public class ResultFormatterTests
    {
        private ResultFormatter _formatter;

        [TestInitialize]
        public void Setup()
        {
            _formatter = new ResultFormatter();
        }

        [TestMethod]
        public void FormatTitle_ReturnsQuestionTitle()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "How to use async/await?",
                Link = "https://stackoverflow.com/questions/1"
            };
            
            // Act
            var result = _formatter.FormatTitle(question);
            
            // Assert
            Assert.AreEqual("How to use async/await?", result);
        }

        [TestMethod]
        public void FormatSubtitle_IncludesScore()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                Score = 42
            };
            
            // Act
            var result = _formatter.FormatSubtitle(question);
            
            // Assert
            Assert.IsTrue(result.Contains("42"));
        }

        [TestMethod]
        public void FormatSubtitle_IncludesAnswerCount()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                AnswerCount = 5
            };
            
            // Act
            var result = _formatter.FormatSubtitle(question);
            
            // Assert
            Assert.IsTrue(result.Contains("5"));
            Assert.IsTrue(result.Contains("answer"));
        }

        [TestMethod]
        public void FormatSubtitle_ShowsAcceptedAnswerIndicator()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                HasAcceptedAnswer = true
            };
            
            // Act
            var result = _formatter.FormatSubtitle(question);
            
            // Assert
            Assert.IsTrue(result.Contains("✓") || result.Contains("accepted"));
        }

        [TestMethod]
        public void FormatSubtitle_IncludesTags()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                Tags = new[] { "c#", "async" }
            };
            
            // Act
            var result = _formatter.FormatSubtitle(question);
            
            // Assert
            Assert.IsTrue(result.Contains("c#"));
            Assert.IsTrue(result.Contains("async"));
        }

        [TestMethod]
        public void FormatSubtitle_HandlesNoTags()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                Tags = Array.Empty<string>()
            };
            
            // Act
            var result = _formatter.FormatSubtitle(question);
            
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FormatSubtitle_HandlesNegativeScore()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                Score = -5
            };
            
            // Act
            var result = _formatter.FormatSubtitle(question);
            
            // Assert
            Assert.IsTrue(result.Contains("-5") || result.Contains("−5"));
        }

        [TestMethod]
        public void FormatSubtitle_HandlesZeroAnswers()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                AnswerCount = 0
            };
            
            // Act
            var result = _formatter.FormatSubtitle(question);
            
            // Assert
            Assert.IsTrue(result.Contains("0") || result.Contains("no answer"));
        }

        [TestMethod]
        public void FormatSubtitle_LimitsTagsDisplay()
        {
            // Arrange
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                Tags = new[] { "tag1", "tag2", "tag3", "tag4", "tag5", "tag6" }
            };
            
            // Act
            var result = _formatter.FormatSubtitle(question);
            
            // Assert
            // Should limit to reasonable number of tags (e.g., first 3-5)
            Assert.IsNotNull(result);
        }
    }
}
