using System.ComponentModel.DataAnnotations;
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
    public class FieldDefinitionTests
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
        public void FieldDefinition_ShouldValidateCorrectly()
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
                IsRequired = true
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(fieldDefinition, serviceProvider, null);

            // Act
            var results = fieldDefinition.Validate(context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void FieldDefinition_ShouldFailValidation()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition
            {
                InputDefinition = new InputDefinition
                {
                    Label = new L10nString { ["en"] = "" }
                },
                Type = null,
                Description = new L10nString { ["en"] = "" },
                IsList = false,
                SortKey = -1,
                IsRequired = true
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(fieldDefinition, serviceProvider, null);

            // Act
            var results = fieldDefinition.Validate(context).ToList();

            // Assert
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(FieldDefinition.Type)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(FieldDefinition.Description)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(FieldDefinition.SortKey)));
        }

        [Fact]
        public void FieldDefinition_ShouldSerializeAndDeserializeCorrectly()
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
                Constraints = new List<Constraint>
                        {
                            new StringLengthConstraint { MaxLength = 5 },
                            new RegexConstraint { Regex = @"^\d+$" }
                        }
            };

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            // Act
            var json = JsonSerializer.Serialize(fieldDefinition, options);
            var deserializedFieldDefinition = JsonSerializer.Deserialize<FieldDefinition>(json, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            });

            // Assert
            Assert.NotNull(deserializedFieldDefinition);
            Assert.Equal(fieldDefinition.Type.ToJson(), deserializedFieldDefinition.Type.ToJson());
            Assert.Equal(fieldDefinition.InputDefinition.Label["en"], deserializedFieldDefinition.InputDefinition.Label["en"]);
            Assert.Equal(fieldDefinition.Description["en"], deserializedFieldDefinition.Description["en"]);
            Assert.Equal(fieldDefinition.IsList, deserializedFieldDefinition.IsList);
            Assert.Equal(fieldDefinition.SortKey, deserializedFieldDefinition.SortKey);
            Assert.Equal(fieldDefinition.IsRequired, deserializedFieldDefinition.IsRequired);
            Assert.Equal(fieldDefinition.Constraints.Count, deserializedFieldDefinition.Constraints.Count);
            Assert.IsType<StringLengthConstraint>(deserializedFieldDefinition.Constraints[0]);
            Assert.IsType<RegexConstraint>(deserializedFieldDefinition.Constraints[1]);
        }
        [Fact]
        public void FieldDefinition_WithChoiceOptions_ShouldValidateCorrectly()
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
                ChoiceOptions = new List<ChoiceOption>
                {
                    new ChoiceOption { Value = "Option1", Name = new L10nString { ["en"] = "Option 1" } },
                    new ChoiceOption { Value = "Option2", Name = new L10nString { ["en"] = "Option 2" } }
                }
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(fieldDefinition, serviceProvider, null);

            // Act
            var results = fieldDefinition.Validate(context).ToList();

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void FieldDefinition_WithMultipleConstraints_ShouldValidateCorrectly()
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
                Constraints = new List<Constraint>
                {
                    new StringLengthConstraint { MaxLength = 5 },
                    new RegexConstraint { Regex = @"^\d+$" }
                }
            };

            var mockSupportedCultureService = new Mock<ISupportedCultureService>();
            mockSupportedCultureService.Setup(s => s.GetSupportedCultures()).Returns(new List<string> { "en" });

            var mockStringLocalizer = new Mock<IStringLocalizer<Validations>>();
            mockStringLocalizer.Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, key));

            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockSupportedCultureService.Object)
                .AddSingleton(mockStringLocalizer.Object)
                .BuildServiceProvider();

            var context = new ValidationContext(fieldDefinition, serviceProvider, null);

            // Act
            var results = fieldDefinition.Validate(context).ToList();

            // Assert
            Assert.Empty(results);
        }
    }
}
