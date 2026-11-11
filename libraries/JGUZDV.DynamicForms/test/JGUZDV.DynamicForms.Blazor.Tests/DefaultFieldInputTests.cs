using System.Collections;
using System.ComponentModel.DataAnnotations;

using AngleSharp.Html.Dom;

using Bunit;

using JGUZDV.DynamicForms.Model;
using JGUZDV.DynamicForms.Model.Constraints;
using JGUZDV.DynamicForms.Model.FieldTypes;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JGUZDV.DynamicForms.Blazor.Tests;

public class DefaultFieldInputTests : DefaultFieldInputTestContext
{
    [Theory]
    [InlineData("String")]
    [InlineData("Int")]
    [InlineData("Float")]
    [InlineData("Date")]
    [InlineData("Time")]
    [InlineData("File")]
    public void ScalarInput_ConvertsValueAndNotifies(string name)
    {
        var sample = Sample(name);
        var field = CreateField(sample.Type);
        var changes = new List<Field>();
        var component = RenderField(field, changes);

        component.Find("input").Change(sample.Input);

        Assert.Equal(sample.Value, field.Value);
        Assert.IsType(sample.Type.ClrType, field.Value);
        Assert.Same(field, Assert.Single(changes));
        Assert.Empty(component.FindAll(".text-danger"));
        Assert.Empty(field.Validate(new ValidationContext(field)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CheckboxInput_ConvertsValueAndNotifies(bool value)
    {
        var field = CreateField(BoolFieldType.Instance);
        var changes = new List<Field>();
        var component = RenderField(field, changes);

        component.Find("input[type=checkbox]").Input(new ChangeEventArgs { Value = value });

        Assert.Equal(value, Assert.IsType<bool>(field.Value));
        Assert.Same(field, Assert.Single(changes));
        Assert.Empty(component.FindAll(".text-danger"));
    }

    [Theory]
    [InlineData("String")]
    [InlineData("Int")]
    [InlineData("Float")]
    [InlineData("Bool")]
    [InlineData("Date")]
    [InlineData("Time")]
    [InlineData("File")]
    public void ScalarInput_RendersExistingValueAndInputType(string name)
    {
        var sample = Sample(name);
        var field = CreateField(sample.Type);
        field.Value = sample.Value;
        var component = RenderField(field);

        var input = component.Find("input");
        Assert.Equal(sample.Type.HtmlInputType, input.GetAttribute("type"));
        Assert.Equal(sample.Type.ConvertFromValue(sample.Value), input.GetAttribute("value"));
    }

    [Fact]
    public void ScalarInput_RendersLabelAndTogglesDescription()
    {
        var field = CreateField(StringFieldType.Instance);
        var component = RenderField(field);

        Assert.Contains("Field label", component.Find("label").TextContent);
        Assert.Equal(field.FieldDefinition.InputDefinition.Id, component.Find("label").GetAttribute("for"));
        Assert.Empty(component.FindAll("p"));

        component.Find("label .small").Click();
        Assert.Equal("Field description", component.Find("p").TextContent.Trim());

        component.Find("label .small").Click();
        Assert.Empty(component.FindAll("p"));
    }

    [Theory]
    [InlineData("Int", "not an integer")]
    [InlineData("Int", "")]
    [InlineData("Float", "not a number")]
    [InlineData("Date", "not a date")]
    [InlineData("Time", "not a time")]
    [InlineData("File", "not json")]
    public void ScalarInput_InvalidValueShowsErrorAndValidValueClearsIt(string name, string invalid)
    {
        var sample = Sample(name);
        var field = CreateField(sample.Type);
        field.Value = sample.Value;
        var changes = new List<Field>();
        var component = RenderField(field, changes);

        component.Find("input").Change(invalid);

        Assert.Null(field.Value);
        Assert.Contains("FieldInput.Validation.Convert", Assert.Single(component.FindAll(".text-danger")).TextContent);
        Assert.Same(field, Assert.Single(changes));

        component.Find("input").Change(sample.Input);

        Assert.Equal(sample.Value, field.Value);
        Assert.Empty(component.FindAll(".text-danger"));
        Assert.Equal(2, changes.Count);
    }

    [Theory]
    [InlineData("String")]
    [InlineData("Int")]
    [InlineData("Float")]
    [InlineData("Bool")]
    [InlineData("Date")]
    [InlineData("Time")]
    [InlineData("File")]
    public void ManualList_AddAndRemovePreserveTypedListAndNotify(string name)
    {
        var sample = Sample(name);
        var field = CreateField(sample.Type, isList: true);
        var changes = new List<Field>();
        var component = RenderField(field, changes);

        component.Find("input").Input(sample.Input);
        Assert.Empty(field.Values);
        Assert.Empty(changes);
        component.Find("button.btn-outline-secondary").Click();

        Assert.Equal(typeof(List<>).MakeGenericType(sample.Type.ClrType), field.Value!.GetType());
        Assert.Equal(sample.Value, Assert.Single(field.Values));
        Assert.Equal("", Assert.IsAssignableFrom<IHtmlInputElement>(component.Find("input")).Value);
        Assert.Same(field, Assert.Single(changes));
        Assert.Empty(component.FindAll(".text-danger"));

        component.Find("button.btn-outline-danger").Click();

        Assert.Empty(Assert.IsAssignableFrom<IList>(field.Value));
        Assert.Equal(typeof(List<>).MakeGenericType(sample.Type.ClrType), field.Value.GetType());
        Assert.Equal(2, changes.Count);
    }

    [Fact]
    public void ManualList_EnterAddsValueOtherKeysDoNot()
    {
        var field = CreateField(IntFieldType.Instance, isList: true);
        var component = RenderField(field);
        component.Find("input").Input("42");

        component.Find("input").KeyUp(new KeyboardEventArgs { Code = "Escape" });
        Assert.Empty(field.Values);

        component.Find("input").KeyUp(new KeyboardEventArgs { Code = "Enter" });
        Assert.Equal(42, Assert.Single(field.Values));
    }

    [Theory]
    [InlineData("Int")]
    [InlineData("Float")]
    [InlineData("Bool")]
    [InlineData("Date")]
    [InlineData("Time")]
    [InlineData("File")]
    public void ManualList_InvalidValueClearsListAndReportsErrorThenRecovers(string name)
    {
        var sample = Sample(name);
        var field = CreateField(sample.Type, isList: true);
        ((IList)field.Value!).Add(sample.Value);
        var changes = new List<Field>();
        var component = RenderField(field, changes);

        component.Find("input").Input("invalid");
        component.Find("button.btn-outline-secondary").Click();

        Assert.Empty(field.Values);
        Assert.Equal(typeof(List<>).MakeGenericType(sample.Type.ClrType), field.Value!.GetType());
        Assert.Contains("FieldInput.Validation.Convert", Assert.Single(component.FindAll(".text-danger")).TextContent);
        Assert.Same(field, Assert.Single(changes));

        component.Find("input").Input(sample.Input);
        component.Find("button.btn-outline-secondary").Click();

        Assert.Equal(sample.Value, Assert.Single(field.Values));
        Assert.Empty(component.FindAll(".text-danger"));
        Assert.Equal(2, changes.Count);
    }

    [Fact]
    public void ScalarInput_UpdatedValueCanBeValidatedAgainstModelConstraints()
    {
        var field = CreateField(StringFieldType.Instance);
        field.FieldDefinition.IsRequired = true;
        field.FieldDefinition.Constraints.Add(new StringLengthConstraint { MaxLength = 5 });
        var component = RenderField(field);

        component.Find("input").Change("Too long");
        Assert.Contains(field.Validate(new ValidationContext(field)), error => error.ErrorMessage == "Constraint.Validation.Length");

        component.Find("input").Change("Valid");
        Assert.Empty(field.Validate(new ValidationContext(field)));
    }
}
