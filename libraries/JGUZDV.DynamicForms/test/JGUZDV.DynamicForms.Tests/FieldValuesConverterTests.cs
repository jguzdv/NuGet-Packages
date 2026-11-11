using System.Text.Json;

using JGUZDV.DynamicForms.Model;

namespace JGUZDV.DynamicForms.Tests;

public class FieldValuesConverterTests
{
    [Fact]
    public void FieldValues_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var fieldValues = new FieldValues(new Dictionary<FieldId, object?>
        {
            [new FieldId("Name")] = "Test",
            [new FieldId("Count")] = 42,
            [new FieldId("Enabled")] = true,
            [new FieldId("Optional")] = null,
            [new FieldId("List")] = new List<string> { "Item1", "Item2" }
        });

        // Act
        var json = JsonSerializer.Serialize(fieldValues);
        var deserializedFieldValues = JsonSerializer.Deserialize<FieldValues>(json);

        // Assert
        Assert.Equal("{\"Values\":[{\"Name\":\"Test\"},{\"Count\":42},{\"Enabled\":true},{\"Optional\":null},{\"List\":[\"Item1\",\"Item2\"]}]}", json);
        Assert.NotNull(deserializedFieldValues);
        Assert.Equal("Test", Assert.IsType<JsonElement>(deserializedFieldValues.Values[new FieldId("Name")]).GetString());
        Assert.Equal(42, Assert.IsType<JsonElement>(deserializedFieldValues.Values[new FieldId("Count")]).GetInt32());
        Assert.True(Assert.IsType<JsonElement>(deserializedFieldValues.Values[new FieldId("Enabled")]).GetBoolean());
        Assert.Equal(JsonValueKind.Null, Assert.IsType<JsonElement>(deserializedFieldValues.Values[new FieldId("Optional")]).ValueKind);
        var list = Assert.IsType<JsonElement[]>(deserializedFieldValues.Values[new FieldId("List")]);
        Assert.Equal(new[] { "Item1", "Item2" }, list.Select(item => item.GetString()));
    }
}
