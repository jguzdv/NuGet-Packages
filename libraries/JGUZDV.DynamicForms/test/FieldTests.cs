using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Resources;
using JGUZDV.DynamicForms.Serialization;
using JGUZDV.L10n;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Moq;

using Xunit;

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
                    Type = GetFieldType("StringFieldType").ToJson(),
                    Label = new L10nString { ["en"] = "Test Label" }
                },
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
                    Type = GetFieldType("StringFieldType").ToJson(),
                    Label = new L10nString { ["en"] = "Test Label" }
                },
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
                    Type = GetFieldType("StringFieldType").ToJson(),
                    Label = new L10nString { ["en"] = "Test Label" }
                },
                Description = new L10nString { ["en"] = "Test Description" },
                IsList = false,
                SortKey = 1,
                IsRequired = true
            };

            var field = new Field(fieldDefinition)
            {
                Value = "Test"
            };

            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultResolver(),
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            options.Converters.Add(new FieldConverter());

            // Act
            var json = JsonSerializer.Serialize(field, options);
            var deserializedField = JsonSerializer.Deserialize<Field>(json, options);

            // Assert
            Assert.NotNull(deserializedField);
            Assert.Equal(field.Value, deserializedField.Value);
            Assert.Equal(field.FieldDefinition.InputDefinition.Type, deserializedField.FieldDefinition.InputDefinition.Type);
            Assert.Equal(field.FieldDefinition.InputDefinition.Label["en"], deserializedField.FieldDefinition.InputDefinition.Label["en"]);
            Assert.Equal(field.FieldDefinition.Description["en"], deserializedField.FieldDefinition.Description["en"]);
            Assert.Equal(field.FieldDefinition.IsList, deserializedField.FieldDefinition.IsList);
            Assert.Equal(field.FieldDefinition.SortKey, deserializedField.FieldDefinition.SortKey);
            Assert.Equal(field.FieldDefinition.IsRequired, deserializedField.FieldDefinition.IsRequired);
        }
    }
}

