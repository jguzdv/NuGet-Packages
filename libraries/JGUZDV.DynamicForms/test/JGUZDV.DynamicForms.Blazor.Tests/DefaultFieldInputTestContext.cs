using Bunit;

using JGUZDV.DynamicForms.Blazor.FieldComponents;
using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace JGUZDV.DynamicForms.Blazor.Tests;

public abstract class DefaultFieldInputTestContext : BunitContext
{
    protected DefaultFieldInputTestContext()
    {
        Services.AddLogging();
        Services.AddSingleton(typeof(IStringLocalizer<>), typeof(KeyLocalizer<>));
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    protected IRenderedComponent<DefaultFieldInput> RenderField(Field field, List<Field>? changes = null)
    {
        var component = Render<DefaultFieldInput>(parameters => parameters
            .Add(input => input.Field, field)
            .Add(input => input.FieldChanged, changed => changes?.Add(changed)));
        component.WaitForAssertion(() => Assert.NotEmpty(component.FindAll("input, .multi-select")));
        return component;
    }

    protected static Field CreateField(FieldType type, bool isList = false)
    {
        return new Field(new FieldDefinition
        {
            Type = type,
            IsList = isList,
            Description = new L10nString { ["en"] = "Field description", ["de"] = "Field description" },
            InputDefinition = new InputDefinition
            {
                Id = "test-field",
                Name = "test-field",
                Label = new L10nString { ["en"] = "Field label", ["de"] = "Field label" }
            }
        });
    }

    protected static (FieldType Type, string Input, object Value) Sample(string name) => name switch
    {
        "String" => (StringFieldType.Instance, "Example", "Example"),
        "Int" => (IntFieldType.Instance, "42", 42),
        "Float" => (FloatFieldType.Instance, "42", 42f),
        "Bool" => (BoolFieldType.Instance, "true", true),
        "Date" => (DateOnlyFieldType.Instance, "2026-03-15", new DateOnly(2026, 3, 15)),
        "Time" => (TimeOnlyFieldType.Instance, "13:45:00", new TimeOnly(13, 45)),
        "File" => (FileFieldType.Instance, "{\"FileName\":\"test.txt\",\"FileSize\":12}",
            new FileFieldType.FileInfo { FileName = "test.txt", FileSize = 12 }),
        _ => throw new ArgumentOutOfRangeException(nameof(name))
    };

    public sealed class KeyLocalizer<T> : IStringLocalizer<T>
    {
        public LocalizedString this[string name] => new(name, name);
        public LocalizedString this[string name, params object[] arguments] => new(name, name);
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
    }
}
