using JGUZDV.DynamicForms.Blazor;
using JGUZDV.DynamicForms.Blazor.Fields;
using JGUZDV.DynamicForms.Model;

using Microsoft.AspNetCore.Components;

namespace JGUZDV.DynamicForms;

/// <summary>
/// Helper to register custom input components for field types and constraints in the Dynamic Forms framework.
/// </summary>
public static class DynamicFormsBuilderExtensions
{

    /// <summary>
    /// Sets the input component type for the specified field type.
    /// </summary>
    /// <typeparam name="TFieldType">The type of the field to associate. Must derive from <see cref="FieldType"/>.</typeparam>
    /// <typeparam name="TComponentType">The type of the input component to associate with the field. Must derive from <see cref="ComponentBase"/>.</typeparam>
    /// <param name="builder">The <see cref="DynamicFormsBuilder"/> instance to configure.</param>
    /// <returns></returns>
    public static DynamicFormsBuilder SetInputComponentType<TFieldType, TComponentType>(this DynamicFormsBuilder builder)
        where TFieldType : FieldType
        where TComponentType : ComponentBase
    {
        FieldInputFactory.SetViewType<TFieldType, TComponentType>();
        return builder;
    }

    /// <summary>
    /// Sets the input component type for the specified constraint type 
    /// </summary>
    /// <typeparam name="TConstraint">The type of the constraint to associate with the input component. Must derive from <see cref="Constraint"/>.</typeparam>
    /// <typeparam name="TComponent">The type of the input component to associate with the constraint. Must derive from <see cref="ComponentBase"/>.</typeparam>
    /// <param name="builder">The <see cref="DynamicFormsBuilder"/> instance to configure.</param>
    /// <returns></returns>
    public static DynamicFormsBuilder SetConstraintInputType<TConstraint, TComponent>(this DynamicFormsBuilder builder)
        where TConstraint : Constraint
        where TComponent : ComponentBase
    {
        ConstraintViewTypeFactory.SetViewType(typeof(TConstraint), typeof(TComponent));
        return builder;
    }

    /// <summary>
    /// Sets the input component type for the specified constraint type.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="constraintType"></param>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public static DynamicFormsBuilder SetConstraintInputType(this DynamicFormsBuilder builder, Type constraintType, Type componentType)
    {
        ConstraintViewTypeFactory.SetViewType(constraintType, componentType);
        return builder;
    }
}
