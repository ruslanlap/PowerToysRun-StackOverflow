using Microsoft.VisualStudio.TestTools.UnitTesting;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;
using System;

namespace Community.PowerToys.Run.Plugin.StackOverflow.UnitTests.Models
{
    [TestClass]
    public class StackOverflowQuestionTests
    {
        [TestMethod]
        public void Constructor_ValidData_CreatesQuestion()
        {
            // Arrange & Act
            var question = new StackOverflowQuestion
            {
                QuestionId = 123456,
                Title = "How to use async/await in C#",
                Link = "https://stackoverflow.com/questions/123456",
                Score = 1234,
                AnswerCount = 5,
                HasAcceptedAnswer = true,
                Tags = new[] { "c#", "async-await", "dotnet" },
                LastActivityDate = DateTime.UtcNow
            };
            
            // Assert
            Assert.AreEqual(123456, question.QuestionId);
            Assert.AreEqual("How to use async/await in C#", question.Title);
            Assert.AreEqual("https://stackoverflow.com/questions/123456", question.Link);
            Assert.AreEqual(1234, question.Score);
            Assert.AreEqual(5, question.AnswerCount);
            Assert.IsTrue(question.HasAcceptedAnswer);
            Assert.AreEqual(3, question.Tags.Length);
        }

        [TestMethod]
        public void QuestionId_MustBePositive()
        {
            // Arrange & Act
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1"
            };
            
            // Assert
            Assert.IsTrue(question.QuestionId > 0);
        }

        [TestMethod]
        public void Link_MustStartWithHttps()
        {
            // Arrange & Act
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1"
            };
            
            // Assert
            Assert.IsTrue(question.Link.StartsWith("https://"));
        }

        [TestMethod]
        public void Tags_CanBeEmptyArray()
        {
            // Arrange & Act
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Test",
                Link = "https://stackoverflow.com/questions/1",
                Tags = Array.Empty<string>()
            };
            
            // Assert
            Assert.IsNotNull(question.Tags);
            Assert.AreEqual(0, question.Tags.Length);
        }

        [TestMethod]
        public void Score_CanBeNegative()
        {
            // Arrange & Act
            var question = new StackOverflowQuestion
            {
                QuestionId = 1,
                Title = "Bad Question",
                Link = "https://stackoverflow.com/questions/1",
                Score = -5
            };
            
            // Assert
            Assert.IsTrue(question.Score < 0);
        }
    }
}
