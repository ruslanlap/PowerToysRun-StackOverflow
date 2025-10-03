using Microsoft.VisualStudio.TestTools.UnitTesting;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;

namespace Community.PowerToys.Run.Plugin.StackOverflow.UnitTests.Models
{
    [TestClass]
    public class SearchQueryTests
    {
        [TestMethod]
        public void ValidQuery_WithValidInput_SetsIsValidTrue()
        {
            // Arrange & Act
            var query = SearchQuery.Create("python lists");
            
            // Assert
            Assert.IsTrue(query.IsValid);
            Assert.IsNull(query.ValidationError);
        }

        [TestMethod]
        public void InvalidQuery_TooShort_SetsIsValidFalse()
        {
            // Arrange & Act
            var query = SearchQuery.Create("p");
            
            // Assert
            Assert.IsFalse(query.IsValid);
            Assert.IsNotNull(query.ValidationError);
        }

        [TestMethod]
        public void InvalidQuery_TooLong_SetsIsValidFalse()
        {
            // Arrange
            var longQuery = new string('a', 201);
            
            // Act
            var query = SearchQuery.Create(longQuery);
            
            // Assert
            Assert.IsFalse(query.IsValid);
            Assert.IsNotNull(query.ValidationError);
        }

        [TestMethod]
        public void InvalidQuery_NullOrEmpty_SetsIsValidFalse()
        {
            // Arrange & Act
            var nullQuery = SearchQuery.Create(null);
            var emptyQuery = SearchQuery.Create("");
            
            // Assert
            Assert.IsFalse(nullQuery.IsValid);
            Assert.IsFalse(emptyQuery.IsValid);
        }

        [TestMethod]
        public void InvalidQuery_WhitespaceOnly_SetsIsValidFalse()
        {
            // Arrange & Act
            var query = SearchQuery.Create("   ");
            
            // Assert
            Assert.IsFalse(query.IsValid);
            Assert.IsNotNull(query.ValidationError);
        }

        [TestMethod]
        public void NormalizedQuery_TrimsAndLowercases()
        {
            // Arrange & Act
            var query = SearchQuery.Create("  Python Lists  ");
            
            // Assert
            Assert.AreEqual("python lists", query.NormalizedQuery);
            Assert.AreEqual("  Python Lists  ", query.RawQuery);
        }

        [TestMethod]
        public void ValidationError_SetForInvalidQuery()
        {
            // Arrange & Act
            var query = SearchQuery.Create("x");
            
            // Assert
            Assert.IsFalse(query.IsValid);
            Assert.IsTrue(query.ValidationError.Contains("at least"));
        }
    }
}
