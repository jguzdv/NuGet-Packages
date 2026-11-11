using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Model.FieldTypes;

namespace JGUZDV.DynamicForms.Tests;

public class FieldCollectionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CreateFromSchemaAndValues_AfterJsonRoundTrip_ShouldCreateValidFields(bool emptyLists)
    {
        // Arrange
        var nameId = new FieldId("Name");
        var countId = new FieldId("Count");
        var tagsId = new FieldId("Tags");
        var numbersId = new FieldId("Numbers");
        var schema = new FieldSchema(new List<FieldDefinition>
        {
            new()
            {
                Identifier = nameId,
                Type = StringFieldType.Instance,
                IsRequired = true,
                Constraints = new List<IConstraint> { new StringLengthConstraint { MaxLength = 10 } }
            },
            new()
            {
                Identifier = countId,
                Type = IntFieldType.Instance,
                IsRequired = true,
                Constraints = new List<IConstraint>
                {
                    new RangeConstraint { FieldType = IntFieldType.Instance, MinValue = 1, MaxValue = 100 }
                }
            },
            new()
            {
                Identifier = tagsId,
                Type = StringFieldType.Instance,
                IsList = true,
                Constraints = new List<IConstraint> { new StringLengthConstraint { MaxLength = 10 } }
            },
            new()
            {
                Identifier = numbersId,
                Type = IntFieldType.Instance,
                IsList = true,
                Constraints = new List<IConstraint>
                {
                    new RangeConstraint { FieldType = IntFieldType.Instance, MinValue = 1, MaxValue = 100 }
                }
            }
        });
        var tags = emptyLists ? new List<string>() : new List<string> { "Item1", "Item2" };
        var numbers = emptyLists ? new List<int>() : new List<int> { 1, 42 };
        var fieldValues = new FieldValues(new Dictionary<FieldId, object?>
        {
            [nameId] = "Test",
            [countId] = 42,
            [tagsId] = tags,
            [numbersId] = numbers
        });

        // Act
        var json = JsonSerializer.Serialize(fieldValues);
        var deserializedValues = JsonSerializer.Deserialize<FieldValues>(json);
        Assert.NotNull(deserializedValues);
        var collection = FieldCollection.CreateFromSchemaAndValues(schema, deserializedValues);

        // Assert
        Assert.Equal(schema.FieldDefinitions.Count, collection.Fields.Count);
        var fields = collection.Fields.ToDictionary(field => field.FieldDefinition.Identifier);
        Assert.Equal("Test", Assert.IsType<string>(fields[nameId].Value));
        Assert.Equal(42, Assert.IsType<int>(fields[countId].Value));
        Assert.Equal(tags, Assert.IsType<List<string>>(fields[tagsId].Value));
        Assert.Equal(numbers, Assert.IsType<List<int>>(fields[numbersId].Value));
        Assert.All(collection.Fields, field =>
        {
            Assert.Same(schema.AsDictionary()[field.FieldDefinition.Identifier], field.FieldDefinition);
            Assert.Empty(field.Validate(new ValidationContext(field)));
        });
    }
}
