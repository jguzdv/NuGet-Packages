using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.DynamicForms.Resources;
using JGUZDV.L10n;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Moq;

namespace JGUZDV.DynamicForms.Tests
{
    public class FieldTests
    {
        [Theory]
        [InlineData("[1, 42]", new int[] { 1, 42 })]
        [InlineData("[]", new int[] { })]
        public void Field_WithIntList_ShouldConvertJsonArrayString(string json, int[] expected)
        {
            // Arrange
            var field = new Field(new FieldDefinition { Type = IntFieldType.Instance, IsList = true });

            // Act
            field.Value = json;

            // Assert
            Assert.Equal(expected, Assert.IsType<List<int>>(field.Value));
            Assert.Empty(field.Validate(new ValidationContext(field)));
        }

        [Theory]
        [InlineData("[\"Item1\", \"Item2\"]", new string[] { "Item1", "Item2" })]
        [InlineData("[]", new string[] { })]
        public void Field_WithStringList_ShouldConvertJsonArrayString(string json, string[] expected)
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Type = StringFieldType.Instance, IsList = true };

            // Act
            var field = new Field(fieldDefinition, json);

            // Assert
            Assert.Equal(expected, Assert.IsType<List<string>>(field.Value));
            Assert.Empty(field.Validate(new ValidationContext(field)));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Field_WithIntList_ShouldPreserveTypedList(bool hasChoices)
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Type = IntFieldType.Instance, IsList = true };
            if (hasChoices)
            {
                fieldDefinition.ChoiceOptions.Add(new ChoiceOption { Value = "1" });
                fieldDefinition.ChoiceOptions.Add(new ChoiceOption { Value = "42" });
            }
            var field = new Field(fieldDefinition);
            var values = new List<int> { 1, 42 };

            // Act
            field.Value = values;

            // Assert
            Assert.Same(values, field.Value);
            Assert.Empty(field.Validate(new ValidationContext(field)));
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void Field_WithIntType_ShouldRejectStringList(bool isList, bool hasChoices)
        {
            // Arrange
            var fieldDefinition = new FieldDefinition { Type = IntFieldType.Instance, IsList = isList };
            if (hasChoices)
            {
                fieldDefinition.ChoiceOptions.Add(new ChoiceOption { Value = "42" });
            }
            var field = new Field(fieldDefinition);
            var originalValue = field.Value;
            var values = new List<string> { "42" };

            // Act
            Assert.Throws<InvalidOperationException>(() => field.Value = values);

            // Assert
            Assert.Same(originalValue, field.Value);
            Assert.Throws<InvalidOperationException>(() => new Field(fieldDefinition, values));
        }

        [Theory]
        [InlineData("42")]
        [InlineData("[\"not an integer\"]")]
        public void Field_WithIntList_ShouldRejectInvalidJsonArrayString(string json)
        {
            // Arrange
            var field = new Field(new FieldDefinition { Type = IntFieldType.Instance, IsList = true });
            var originalValue = field.Value;

            // Act
            Assert.Throws<JsonException>(() => field.Value = json);

            // Assert
            Assert.Same(originalValue, field.Value);
        }

        [Fact]
        public void Field_WithIntList_ShouldRejectJsonNullString()
        {
            // Arrange
            var field = new Field(new FieldDefinition { Type = IntFieldType.Instance, IsList = true });
            var originalValue = field.Value;

            // Act
            Assert.Throws<DynamicFormsException>(() => field.Value = "null");

            // Assert
            Assert.Same(originalValue, field.Value);
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
                Type = StringFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<IConstraint> { new StringLengthConstraint { MaxLength = 5 } }
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
                Type = StringFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<IConstraint> { new StringLengthConstraint { MaxLength = 5 } }
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
                Type = StringFieldType.Instance,
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
            Assert.Equal(field.FieldDefinition.Type!.ToJson(), deserializedField.FieldDefinition.Type!.ToJson());
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
                Type = DateOnlyFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Date Description" },
                IsList = false,
                SortKey = 2,
                IsRequired = true,
                Constraints = new List<IConstraint>
                {
                    new RangeConstraint
                    {
                        MinValue = new DateOnly(2020, 1, 1),
                        MaxValue = new DateOnly(2025, 12, 31),
                        FieldType = DateOnlyFieldType.Instance
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
            Assert.Equal(field.FieldDefinition.Type!.ToJson(), deserializedField.FieldDefinition.Type!.ToJson());
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
                Type = StringFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<IConstraint> { new RegexConstraint { Regex = @"^\d+$" } }
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
                Type = StringFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = true,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<IConstraint> { new SizeConstraint { MinCount = 1, MaxCount = 3 } }
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
                Type = FileFieldType.Instance,
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<IConstraint> { new FileSizeConstraint { MaxFileSize = 1024 } }
            };

            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileInfo { FileName = "file1.txt", FileSize = 512 }
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
                Type = FileFieldType.Instance,
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<IConstraint> { new FileSizeConstraint { MaxFileSize = 1024 } }
            };

            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileInfo { FileName = "file1.txt", FileSize = 2048 }
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
                Type = FileFieldType.Instance,
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("Test file content"));
            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileInfo { FileName = "file1.txt", FileSize = fileContent.Length, Stream = fileContent }
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
            Assert.Equal(((FileFieldType.FileInfo)field.Value).FileName, ((FileFieldType.FileInfo)deserializedField.Value!).FileName);
            Assert.Equal(((FileFieldType.FileInfo)field.Value).FileSize, ((FileFieldType.FileInfo)deserializedField.Value).FileSize);

            // Compare the content of the streams
            using var originalStream = ((FileFieldType.FileInfo)field.Value).Stream;
            using var deserializedStream = ((FileFieldType.FileInfo)deserializedField.Value).Stream;
            Assert.NotNull(originalStream);
            Assert.NotNull(deserializedStream);

            using var originalReader = new StreamReader(originalStream);
            using var deserializedReader = new StreamReader(deserializedStream);
            var originalContent = originalReader.ReadToEnd();
            var deserializedContent = deserializedReader.ReadToEnd();
            Assert.Equal(originalContent, deserializedContent);

            Assert.Equal(field.FieldDefinition.Type!.ToJson(), deserializedField.FieldDefinition.Type!.ToJson());
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
                Type = FileFieldType.Instance,
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<IConstraint> { new FileSizeConstraint { MaxFileSize = 1024 } }
            };

            var fileContent = new MemoryStream(new byte[512]);
            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileInfo { FileName = "file1.txt", FileSize = fileContent.Length, Stream = fileContent }
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
                Type = FileFieldType.Instance,
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true,
                Constraints = new List<IConstraint> { new FileSizeConstraint { MaxFileSize = 1024 } }
            };

            var fileContent = new MemoryStream(new byte[2048]);
            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileInfo { FileName = "file1.txt", FileSize = fileContent.Length, Stream = fileContent }
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
                Type = FileFieldType.Instance,
                Description = new L10nString { ["en"] = "Test File Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var originalContent = "Test file content";
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
            var field = new Field(fieldDefinition)
            {
                Value = new FileFieldType.FileInfo { FileName = "file1.txt", FileSize = fileContent.Length, Stream = fileContent }
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

        [Fact]
        public async Task Field_WithDateOnlyFieldTypeAndNullValue_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Date Label" }
                },
                Type = DateOnlyFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Date Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var field = new Field(fieldDefinition);

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };


            // Act
            var json = JsonSerializer.Serialize(field, options);
            var deserializedField = JsonSerializer.Deserialize<Field>(json, options);

            // Assert
            Assert.NotNull(deserializedField);
            Assert.Null(deserializedField.Value);
        }

        [Fact]
        public async Task Field_WithIntFieldTypeAndNullValue_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Int Label" }
                },
                Type = IntFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Int Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var field = new Field(fieldDefinition);

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };


            // Act
            var json = JsonSerializer.Serialize(field, options);
            var deserializedField = JsonSerializer.Deserialize<Field>(json, options);

            // Assert
            Assert.NotNull(deserializedField);
            Assert.Null(deserializedField.Value);
        }

        [Fact]
        public async Task Field_WithBoolFieldTypeAndNullValue_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Bool Label" }
                },
                Type = BoolFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Bool Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var field = new Field(fieldDefinition);

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };


            // Act
            var json = JsonSerializer.Serialize(field, options);
            var deserializedField = JsonSerializer.Deserialize<Field>(json, options);

            // Assert
            Assert.NotNull(deserializedField);
            Assert.Null(deserializedField.Value);
        }

        [Fact]
        public async Task Field_WithTimeFieldTypeAndNullValue_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "Test Time Label" }
                },
                Type = TimeOnlyFieldType.Instance,
                Description = new L10nString { ["en"] = "Test Time Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var field = new Field(fieldDefinition);

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };


            // Act
            var json = JsonSerializer.Serialize(field, options);
            var deserializedField = JsonSerializer.Deserialize<Field>(json, options);

            // Assert
            Assert.NotNull(deserializedField);
            Assert.Null(deserializedField.Value);
        }
    }
}

