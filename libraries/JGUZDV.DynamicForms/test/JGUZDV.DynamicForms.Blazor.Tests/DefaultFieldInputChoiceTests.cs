using System.Collections;
using System.ComponentModel.DataAnnotations;

using Bunit;

using JGUZDV.Blazor.Components.Inputs;
using JGUZDV.DynamicForms.Blazor.FieldComponents;
using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Model.FieldTypes;
using JGUZDV.L10n;

using Microsoft.Extensions.DependencyInjection;

namespace JGUZDV.DynamicForms.Blazor.Tests;

public class DefaultFieldInputChoiceTests : DefaultFieldInputTestContext
{
    [Theory]
    [InlineData("String")]
    [InlineData("Int")]
    [InlineData("Float")]
    [InlineData("Bool")]
    [InlineData("Date")]
    [InlineData("Time")]
    [InlineData("File")]
    public async Task MultiChoice_SelectAndClearCreateTypedListsAndNotify(string name)
    {
        var sample = Sample(name);
        var field = CreateField(sample.Type, isList: true);
        var choice = Choice(sample.Type.ConvertFromValue(sample.Value), "Choice");
        field.FieldDefinition.ChoiceOptions.Add(choice);
        var changes = new List<Field>();
        var component = RenderField(field, changes);
        var selector = component.FindComponent<MultiSelect<ChoiceOption, string>>();
        Assert.False(selector.Instance.UseSingleSelectionMode);
        Assert.Empty(selector.Instance.Values!);

        await component.InvokeAsync(() => selector.Instance.ValuesChanged.InvokeAsync(new List<ChoiceOption> { choice }));

        Assert.Equal(typeof(List<>).MakeGenericType(sample.Type.ClrType), field.Value!.GetType());
        Assert.Equal(sample.Value, Assert.Single(field.Values));
        Assert.Same(choice, Assert.Single(selector.Instance.Values!));
        Assert.Same(field, Assert.Single(changes));
        Assert.Empty(field.Validate(new ValidationContext(field)));

        await component.InvokeAsync(() => selector.Instance.ValuesChanged.InvokeAsync(new List<ChoiceOption>()));

        Assert.Equal(typeof(List<>).MakeGenericType(sample.Type.ClrType), field.Value.GetType());
        Assert.Empty(Assert.IsAssignableFrom<IList>(field.Value));
        Assert.Empty(selector.Instance.Values!);
        Assert.Equal(2, changes.Count);
        Assert.Empty(component.FindAll(".text-danger"));
        Assert.Empty(field.Validate(new ValidationContext(field)));
    }

    [Theory]
    [InlineData("String")]
    [InlineData("Int")]
    [InlineData("Float")]
    [InlineData("Bool")]
    [InlineData("Date")]
    [InlineData("Time")]
    [InlineData("File")]
    public async Task SingleChoice_SelectConvertsValueAndNotifies(string name)
    {
        var sample = Sample(name);
        var field = CreateField(sample.Type);
        var choice = Choice(sample.Type.ConvertFromValue(sample.Value), "Choice");
        field.FieldDefinition.ChoiceOptions.Add(choice);
        var changes = new List<Field>();
        var component = RenderField(field, changes);
        var selector = component.FindComponent<MultiSelect<ChoiceOption, string>>();
        Assert.True(selector.Instance.UseSingleSelectionMode);
        Assert.Null(selector.Instance.Value);

        await component.InvokeAsync(() => selector.Instance.ValueChanged.InvokeAsync(choice));

        Assert.Equal(sample.Value, field.Value);
        Assert.IsType(sample.Type.ClrType, field.Value);
        Assert.Same(choice, selector.Instance.Value);
        Assert.Same(field, Assert.Single(changes));
        Assert.Empty(field.Validate(new ValidationContext(field)));
    }

    [Fact]
    public async Task SingleChoice_ChangingSelectionReplacesValueAndNotifies()
    {
        var field = CreateField(IntFieldType.Instance);
        var choice = Choice("42", "Forty-two");
        var replacement = Choice("1", "One");
        field.FieldDefinition.ChoiceOptions.AddRange([choice, replacement]);
        field.Value = 42;
        var changes = new List<Field>();
        var component = RenderField(field, changes);
        var selector = component.FindComponent<MultiSelect<ChoiceOption, string>>();
        Assert.Same(choice, selector.Instance.Value);

        await component.InvokeAsync(() => selector.Instance.ValueChanged.InvokeAsync(replacement));

        Assert.Equal(1, Assert.IsType<int>(field.Value));
        Assert.Same(replacement, selector.Instance.Value);
        Assert.Same(field, Assert.Single(changes));
        Assert.Empty(field.Validate(new ValidationContext(field)));
    }

    [Fact]
    public void MultiChoice_ClickingChoicesAndBadgesSelectsAndClears()
    {
        var field = CreateField(IntFieldType.Instance, isList: true);
        field.FieldDefinition.ChoiceOptions.Add(Choice("1", "One"));
        field.FieldDefinition.ChoiceOptions.Add(Choice("42", "Forty-two"));
        var changes = new List<Field>();
        var component = RenderField(field, changes);

        component.FindAll(".multi-select-overlay .col .button")[0].Click();
        component.FindAll(".multi-select-overlay .col .button")[1].Click();

        Assert.Equal(new[] { 1, 42 }, Assert.IsType<List<int>>(field.Value));
        Assert.Equal(2, component.FindAll(".badge").Count);

        component.FindAll(".badge")[0].Click();
        Assert.Equal(new[] { 42 }, Assert.IsType<List<int>>(field.Value));
        component.Find(".badge").Click();

        Assert.Empty(Assert.IsType<List<int>>(field.Value));
        Assert.Empty(component.FindAll(".badge"));
        Assert.Equal(4, changes.Count);
    }

    [Fact]
    public void Choices_AreSortedAndExistingSelectionsAreDisplayed()
    {
        var field = CreateField(IntFieldType.Instance, isList: true);
        var later = Choice("42", "Later", sortKey: 2);
        var earlier = Choice("1", "Earlier", sortKey: 1);
        field.FieldDefinition.ChoiceOptions.AddRange([later, earlier]);
        field.Value = new List<int> { 42 };
        var component = RenderField(field);
        var selector = component.FindComponent<MultiSelect<ChoiceOption, string>>();

        Assert.Equal(new[] { earlier, later }, selector.Instance.Items);
        Assert.Same(later, Assert.Single(selector.Instance.Values!));
        Assert.Contains("Later", component.Find(".badge").TextContent);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Choice_InvalidConversionDoesNotReplaceValueOrNotify(bool isList)
    {
        var field = CreateField(IntFieldType.Instance, isList);
        var invalid = Choice("invalid", "Invalid");
        field.FieldDefinition.ChoiceOptions.Add(invalid);
        var originalValue = field.Value;
        var changes = new List<Field>();
        var component = RenderField(field, changes);
        var selector = component.FindComponent<MultiSelect<ChoiceOption, string>>();

        await Assert.ThrowsAsync<FormatException>(() => component.InvokeAsync(() => isList
            ? selector.Instance.ValuesChanged.InvokeAsync(new List<ChoiceOption> { invalid })
            : selector.Instance.ValueChanged.InvokeAsync(invalid)));

        Assert.Same(originalValue, field.Value);
        Assert.Empty(changes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ValueProvider_LoadsChoicesUsingTypeAndMetadata(bool isList)
    {
        var field = CreateField(IntFieldType.Instance, isList);
        field.FieldDefinition.Metadata = "provider-metadata";
        var choices = new List<ChoiceOption> { Choice("42", "Provided choice") };
        var completion = new TaskCompletionSource<List<ChoiceOption>>(TaskCreationOptions.RunContinuationsAsynchronously);
        var provider = new TestValueProvider(completion.Task);
        Services.AddKeyedSingleton<IFieldTypeValueProvider>(nameof(IntFieldType), provider);
        var component = Render<DefaultFieldInput>(parameters => parameters.Add(input => input.Field, field));

        Assert.Same(field.ValueType, provider.RequestedType);
        Assert.Equal("provider-metadata", provider.RequestedMetadata);
        completion.SetResult(choices);

        component.WaitForAssertion(() =>
        {
            var selector = component.FindComponent<MultiSelect<ChoiceOption, string>>();
            Assert.Equal(choices, selector.Instance.Items);
            Assert.Equal(!isList, selector.Instance.UseSingleSelectionMode);
        });
        Assert.Same(choices, field.FieldDefinition.ChoiceOptions);
        Assert.Equal(1, provider.CallCount);
        await completion.Task;
    }

    [Fact]
    public void ValueProvider_EmptyChoicesUsesManualInput()
    {
        var field = CreateField(IntFieldType.Instance, isList: true);
        var provider = new TestValueProvider(Task.FromResult(new List<ChoiceOption>()));
        Services.AddKeyedSingleton<IFieldTypeValueProvider>(nameof(IntFieldType), provider);

        var component = RenderField(field);

        Assert.Empty(component.FindComponents<MultiSelect<ChoiceOption, string>>());
        Assert.Single(component.FindAll("input"));
        Assert.Empty(field.FieldDefinition.ChoiceOptions);
        Assert.Equal(1, provider.CallCount);
    }

    private static ChoiceOption Choice(string value, string name, int sortKey = 0) => new()
    {
        Value = value,
        SortKey = sortKey,
        Name = new L10nString { ["en"] = name, ["de"] = name }
    };

    private sealed class TestValueProvider(Task<List<ChoiceOption>> choices) : IFieldTypeValueProvider
    {
        public FieldType? RequestedType { get; private set; }
        public string? RequestedMetadata { get; private set; }
        public int CallCount { get; private set; }

        public Task<List<ChoiceOption>> GetValues(FieldType type, string? metadata = null)
        {
            RequestedType = type;
            RequestedMetadata = metadata;
            CallCount++;
            return choices;
        }

        public async Task<(bool HandlesType, List<ChoiceOption> AllowedValues)> TryGetValues(FieldType type, string? metadata = null)
            => (true, await GetValues(type, metadata));
    }
}
