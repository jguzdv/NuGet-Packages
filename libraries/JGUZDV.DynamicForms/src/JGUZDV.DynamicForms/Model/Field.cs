﻿using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JGUZDV.DynamicForms.Model;


public class FieldCollection
{
    public required List<Field> Fields { get; set; }
}

public class Field : IValidatableObject
{
    public Field(FieldDefinition fieldDefinition)
    {
        FieldDefinition = fieldDefinition;
        _value = null;
    }

    [JsonConstructor]
    public Field(FieldDefinition fieldDefinition, object? value)
    {
        FieldDefinition = fieldDefinition;
        _value = value;
    }

    public FieldDefinition FieldDefinition { get; set; }

    private object? _value;
    public object? Value
    {
        get => _value;
        set
        {
            if (value != null)
            {
                var valueType = value.GetType();
                if (FieldDefinition.IsList)
                {
                    if (!typeof(IEnumerable).IsAssignableFrom(valueType) 
                        || !valueType.IsGenericType 
                        || valueType.GetGenericArguments()[0] != ValueType.ClrType)
                    {
                        throw new InvalidOperationException("Type does not match");
                    }
                }
                else if (value.GetType() != ValueType.ClrType)
                {
                    throw new InvalidOperationException("Type does not match");
                }
            }
            _value = value;
        }
    }

    [JsonIgnore]
    public List<object>? Values => FieldDefinition.IsList ? Value as List<object> : throw new InvalidOperationException("Only list fields have multiple values");


    [JsonIgnore]
    public FieldType ValueType => FieldType.FromJson(FieldDefinition.InputDefinition.Type); // => Constants.Types.Get(FieldDefinition.InputDefinition.Type);

    public bool IsValid(ValidationContext validationContext) => !Validate(validationContext).Any();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Value == null)
        {
            if (FieldDefinition.IsRequired)
            {
                return new List<ValidationResult>() { new("Required field can not be null", new string[] { FieldDefinition.InputDefinition.Label.ToString() }) };
            }
            else
            {
                return new List<ValidationResult>();
            }
        }

        List<object> val;

        if (FieldDefinition.IsList)
        {
            if (Values == null)
                return new List<ValidationResult>
                {
                    new("Value can not be null", [nameof(Value)])
                };

            val = Values!;
        }
        else
        {
            val = ((IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(ValueType.ClrType))!).OfType<object>().ToList();
            val.Add(Value);
        }

        var errors = FieldDefinition.Constraints.SelectMany(x => x.ValidateConstraint(val, validationContext));

        return errors;
    }
}