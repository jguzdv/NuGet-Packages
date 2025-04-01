using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Resources;
using JGUZDV.L10n;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Moq;

namespace JGUZDV.DynamicForms.Tests
{
    public class FieldTests
    {
        private static FieldType GetFieldType(string typeName)
        {
            return typeName switch
            {
                "StringFieldType" => new StringFieldType(),
                "IntFieldType" => new IntFieldType(),
                "DateOnlyFieldType" => new DateOnlyFieldType(),
                _ => throw new ArgumentException("Invalid field type name", nameof(typeName))
            };
        }

        [Fact]
        public void Field_ShouldValidateCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Label" }
                },
                Type = GetFieldType("StringFieldType"),
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<Constraint> { new StringLengthConstraint { MaxLength = 5 } }
            };

            var field = new Field(fieldDefinition)
            {
                Value = "Test"
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(field, serviceProvider, null);

            // Act
            var results = field.Validate(context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Field_ShouldFailValidation()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Label" }
                },
                Type = GetFieldType("StringFieldType"),
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<Constraint> { new StringLengthConstraint { MaxLength = 5 } }
            };

            var field = new Field(fieldDefinition)
            {
                Value = "TooLongValue"
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(field, serviceProvider, null);

            // Act
            var results = field.Validate(context).ToList();

            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.ErrorMessage == "Constraint.Validation.Length");
        }

        [Fact]
        public void Field_ShouldSerializeAndDeserializeCorrectly()
        {
            //Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Label" }
                },
                Type = GetFieldType("StringFieldType"),
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = true,
                SortKey = 1,
                IsRequired = true
            };

            var field = new Field(fieldDefinition)
            {
                Value = new List<string> { "Test" }
            };

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };


            // Act
            var json = JsonSerializer.Serialize(field, options);
            var deserializedField = JsonSerializer.Deserialize<Field>(json, options);

            // Assert
            Assert.NotNull(deserializedField);
            Assert.Equal(field.Value, deserializedField.Value);
            Assert.Equal(field.FieldDefinition.Type.ToJson(), deserializedField.FieldDefinition.Type.ToJson());
            Assert.Equal(field.FieldDefinition.InputDefinition.Label["en"], deserializedField.FieldDefinition.InputDefinition.Label["en"]);
            Assert.Equal(field.FieldDefinition.Description["en"], deserializedField.FieldDefinition.Description["en"]);
            Assert.Equal(field.FieldDefinition.IsList, deserializedField.FieldDefinition.IsList);
            Assert.Equal(field.FieldDefinition.SortKey, deserializedField.FieldDefinition.SortKey);
            Assert.Equal(field.FieldDefinition.IsRequired, deserializedField.FieldDefinition.IsRequired);
        }

        [Fact]
        public void Field_WithDateOnlyAndRangeConstraint_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Date Label" }
                },
                Type = GetFieldType("DateOnlyFieldType"),
                Description = new L10nString { ["en"] = "Test Date Description" },
                IsList = false,
                SortKey = 2,
                IsRequired = true,
                Constraints = new List<Constraint>
                        {
                            new RangeConstraint
                            {
                                MinValue = new DateOnly(2020, 1, 1),
                                MaxValue = new DateOnly(2025, 12, 31),
                                FieldType = new DateOnlyFieldType()
                            }
                        }
            };

            var field = new Field(fieldDefinition)
            {
                Value = new DateOnly(2023, 6, 15)
            };

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };


            // Act
            var json = JsonSerializer.Serialize(field, options);
            var deserializedField = JsonSerializer.Deserialize<Field>(json, options);

            // Assert
            Assert.NotNull(deserializedField);
            Assert.Equal(field.Value, deserializedField.Value);
            Assert.Equal(field.FieldDefinition.Type.ToJson(), deserializedField.FieldDefinition.Type.ToJson());
            Assert.Equal(field.FieldDefinition.InputDefinition.Label["en"], deserializedField.FieldDefinition.InputDefinition.Label["en"]);
            Assert.Equal(field.FieldDefinition.Description["en"], deserializedField.FieldDefinition.Description["en"]);
            Assert.Equal(field.FieldDefinition.IsList, deserializedField.FieldDefinition.IsList);
            Assert.Equal(field.FieldDefinition.SortKey, deserializedField.FieldDefinition.SortKey);
            Assert.Equal(field.FieldDefinition.IsRequired, deserializedField.FieldDefinition.IsRequired);
            Assert.Equal(((RangeConstraint)field.FieldDefinition.Constraints.First()).MinValue, ((RangeConstraint)deserializedField.FieldDefinition.Constraints.First()).MinValue);
            Assert.Equal(((RangeConstraint)field.FieldDefinition.Constraints.First()).MaxValue, ((RangeConstraint)deserializedField.FieldDefinition.Constraints.First()).MaxValue);
        }
        [Fact]
        public void Field_WithRegexConstraint_ShouldValidateCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Label" }
                },
                Type = GetFieldType("StringFieldType"),
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<Constraint> { new RegexConstraint { Regex = @"^\d+$" } }
            };

            var field = new Field(fieldDefinition)
            {
                Value = "12345"
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(field, serviceProvider, null);

            // Act
            var results = field.Validate(context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Field_WithSizeConstraint_ShouldValidateCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Label" }
                },
                Type = GetFieldType("StringFieldType"),
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = true,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<Constraint> { new SizeConstraint { MinCount = 1, MaxCount = 3 } }
            };

            var field = new Field(fieldDefinition)
            {
                Value = new List<string> { "a", "b" }
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(field, serviceProvider, null);

            // Act
            var results = field.Validate(context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Field_WithFileSizeConstraint_ShouldValidateCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test File Label" }
                },
                Type = new FileFieldType(),
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<Constraint> { new FileSizeConstraint { MaxFileSize = 1024 } }
            };

            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileType { FileName = "file1.txt", FileSize = 512 }
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(field, serviceProvider, null);

            // Act
            var results = field.Validate(context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Field_WithFileSizeConstraint_ShouldFailValidation()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test File Label" }
                },
                Type = new FileFieldType(),
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<Constraint> { new FileSizeConstraint { MaxFileSize = 1024 } }
            };

            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileType { FileName = "file1.txt", FileSize = 2048 }
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(field, serviceProvider, null);

            // Act
            var results = field.Validate(context).ToList();

            // Assert
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Field_WithFileFieldType_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test File Label" }
                },
                Type = new FileFieldType(),
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("Test file content"));
            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileType { FileName = "file1.txt", FileSize = fileContent.Length, Stream = fileContent }
            };

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            // Act
            var json = JsonSerializer.Serialize(field, options);
            var deserializedField = JsonSerializer.Deserialize<Field>(json, options);

            // Assert
            Assert.NotNull(deserializedField);
            Assert.Equal(((FileFieldType.FileType)field.Value).FileName, ((FileFieldType.FileType)deserializedField.Value).FileName);
            Assert.Equal(((FileFieldType.FileType)field.Value).FileSize, ((FileFieldType.FileType)deserializedField.Value).FileSize);

            // Compare the content of the streams
            using var originalStream = ((FileFieldType.FileType)field.Value).Stream;
            using var deserializedStream = ((FileFieldType.FileType)deserializedField.Value).Stream;
            Assert.NotNull(originalStream);
            Assert.NotNull(deserializedStream);

            using var originalReader = new StreamReader(originalStream);
            using var deserializedReader = new StreamReader(deserializedStream);
            var originalContent = originalReader.ReadToEnd();
            var deserializedContent = deserializedReader.ReadToEnd();
            Assert.Equal(originalContent, deserializedContent);

            Assert.Equal(field.FieldDefinition.Type.ToJson(), deserializedField.FieldDefinition.Type.ToJson());
            Assert.Equal(field.FieldDefinition.InputDefinition.Label["en"], deserializedField.FieldDefinition.InputDefinition.Label["en"]);
            Assert.Equal(field.FieldDefinition.Description["en"], deserializedField.FieldDefinition.Description["en"]);
            Assert.Equal(field.FieldDefinition.IsList, deserializedField.FieldDefinition.IsList);
            Assert.Equal(field.FieldDefinition.SortKey, deserializedField.FieldDefinition.SortKey);
            Assert.Equal(field.FieldDefinition.IsRequired, deserializedField.FieldDefinition.IsRequired);
        }

        [Fact]
        public void Field_WithFileFieldType_ShouldValidateFileSizeCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test File Label" }
                },
                Type = new FileFieldType(),
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<Constraint> { new FileSizeConstraint { MaxFileSize = 1024 } }
            };

            var fileContent = new MemoryStream(new byte[512]);
            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileType { FileName = "file1.txt", FileSize = fileContent.Length, Stream = fileContent }
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(field, serviceProvider, null);

            // Act
            var results = field.Validate(context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void Field_WithFileFieldType_ShouldFailFileSizeValidation()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test File Label" }
                },
                Type = new FileFieldType(),
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<Constraint> { new FileSizeConstraint { MaxFileSize = 1024 } }
            };

            var fileContent = new MemoryStream(new byte[2048]);
            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileType { FileName = "file1.txt", FileSize = fileContent.Length, Stream = fileContent }
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(field, serviceProvider, null);

            // Act
            var results = field.Validate(context).ToList();

            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.ErrorMessage == "File size exceeds the maximum allowed size of 1024 bytes.");
        }

        [Fact]
        public async Task Field_WithFileFieldType_ShouldAddToContentCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test File Label" }
                },
                Type = new FileFieldType(),
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var originalContent = "Test file content";
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileType { FileName = "file1.txt", FileSize = fileContent.Length, Stream = fileContent }
            };

            var content = new MultipartFormDataContent();

            // Act
            field.AddToContent(content);

            // Assert
            var fileContentAdded = content.FirstOrDefault(c => c.Headers.ContentDisposition?.FileName == "file1.txt");
            Assert.NotNull(fileContentAdded);
            Assert.IsType<StreamContent>(fileContentAdded);
            var streamContent = (StreamContent)fileContentAdded;
            var addedContent = await streamContent.ReadAsStringAsync();

            Assert.Equal(originalContent, addedContent);
        }
    }
}

