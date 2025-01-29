using System.ComponentModel.DataAnnotations;

using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Tests
{
    public class ConstraintTests
    {
        [Fact]
        public void RegexConstraint_ShouldValidateCorrectly()
        {
            // Arrange
            var regexConstraint = new RegexConstraint { Regex = @"^\d+$" };
            var values = new List<object> { "123", "456" };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = regexConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void RegexConstraint_ShouldFailValidation()
        {
            // Arrange
            var regexConstraint = new RegexConstraint { Regex = @"^\d+$" };
            var values = new List<object> { "123", "abc" };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = regexConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Single(results);
        }

        [Fact]
        public void RangeConstraint_ShouldValidateCorrectly()
        {
            // Arrange
            var rangeConstraint = new RangeConstraint { MinValue = 1, MaxValue = 10 };
            var values = new List<object> { 5, 7 };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = rangeConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void RangeConstraint_ShouldFailValidation()
        {
            // Arrange
            var rangeConstraint = new RangeConstraint { MinValue = 1, MaxValue = 10 };
            var values = new List<object> { 0, 11 };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = rangeConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void SizeConstraint_ShouldValidateCorrectly()
        {
            // Arrange
            var sizeConstraint = new SizeConstraint { MinCount = 1, MaxCount = 3 };
            var values = new List<object> { "a", "b" };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = sizeConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void SizeConstraint_ShouldFailValidation()
        {
            // Arrange
            var sizeConstraint = new SizeConstraint { MinCount = 1, MaxCount = 3 };
            var values = new List<object> { "a", "b", "c", "d" };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = sizeConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Single(results);
        }

        [Fact]
        public void StringLengthConstraint_ShouldValidateCorrectly()
        {
            // Arrange
            var stringLengthConstraint = new StringLengthConstraint { MaxLength = 5 };
            var values = new List<object> { "abc", "de" };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = stringLengthConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void StringLengthConstraint_ShouldFailValidation()
        {
            // Arrange
            var stringLengthConstraint = new StringLengthConstraint { MaxLength = 5 };
            var values = new List<object> { "abcdef", "ghijkl" };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = stringLengthConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Equal(2, results.Count);
        }
    }
}
