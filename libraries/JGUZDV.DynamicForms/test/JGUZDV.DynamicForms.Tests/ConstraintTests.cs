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
        public void RangeConstraint_WithDateOnly_ShouldValidateCorrectly()
        {
            // Arrange
            var rangeConstraint = new RangeConstraint
            {
                MinValue = new DateOnly(2020, 1, 1),
                MaxValue = new DateOnly(2025, 12, 31)
            };
            var values = new List<object> { new DateOnly(2023, 6, 15), new DateOnly(2024, 7, 20) };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = rangeConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Empty(results);
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

        [Fact]
        public void FileSizeConstraint_ShouldValidateCorrectly()
        {
            // Arrange
            var fileSizeConstraint = new FileSizeConstraint { MaxFileSize = 1024 };
            var values = new List<object>
            {
                new FileFieldType.FileType { FileName = "file1.txt", FileSize = 512 },
                new FileFieldType.FileType { FileName = "file2.txt", FileSize = 1024 }
            };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = fileSizeConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void FileSizeConstraint_ShouldFailValidation()
        {
            // Arrange
            var fileSizeConstraint = new FileSizeConstraint { MaxFileSize = 1024 };
            var values = new List<object>
            {
                new FileFieldType.FileType { FileName = "file1.txt", FileSize = 2048 }
            };
            var context = new ValidationContext(new FieldDefinition());

            // Act
            var results = fileSizeConstraint.ValidateConstraint(values, context).ToList();

            // Assert
            Assert.Single(results);
        }
    }
}
