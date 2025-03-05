using JGUZDV.DynamicForms.Blazor;
using JGUZDV.DynamicForms.Blazor.Fields;
using JGUZDV.DynamicForms.Model;

using Microsoft.AspNetCore.Components;

namespace JGUZDV.DynamicForms;

public static class DynamicFormsBuilderExtensions
{
    public static DynamicFormsBuilder SetInputComponentType<TFieldType, TComponentType>(this DynamicFormsBuilder builder)
        where TFieldType : FieldType
        where TComponentType : ComponentBase
    {
        FieldInputFactory.SetViewType<TFieldType, TComponentType>();
        return builder;
    }

    public static DynamicFormsBuilder SetConstraintInputType<TConstraint, TComponent>(this DynamicFormsBuilder builder)
        where TConstraint : Constraint
        where TComponent : ComponentBase
    {
        ConstraintViewTypeFactory.SetViewType(typeof(TConstraint), typeof(TComponent));
        return builder;
    }

    public static DynamicFormsBuilder SetConstraintInputType(this DynamicFormsBuilder builder, Type constraintType, Type componentType)
    {
        ConstraintViewTypeFactory.SetViewType(constraintType, componentType);
        return builder;
    }
}
